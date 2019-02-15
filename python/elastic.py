from elasticsearch import Elasticsearch

class elastic(object):

    def __init__(self):
        self._es = Elasticsearch([{'host': 'localhost', 'port': 9200}])
        if self._es.ping():
            print('Yay Connect')
        else:
            print('Awww it could not connect!')

    def create_index(self, index_name='elastic'):
        created = False
        # index settings
        settings = {
            "mappings": {
                "members": {
                    "dynamic": "strict",
                    "properties": {
                        "folder": {
                            "type": "text"
                        },
                        "filename": {
                            "type": "text"
                        },
                        "text": {
                            "type": "text"
                        },
                    }
                }
            }
        }
        try:
            if not self._es.indices.exists(index_name):
                # Ignore 400 means to ignore "Index Already Exist" error.
                self._es.create(index=index_name, ignore=400, body=settings, doc_type="_doc", id=1) 
                print('Created Index')
            created = True
        except Exception as ex:
            print(str(ex))
        finally:
            return created
    
    def store_record(self, index_name, doc_type, record):
        is_stored = True
        try:
            outcome = self._es.index(index=index_name, doc_type=doc_type, body=record)
            # print(outcome)
        except Exception as ex:
            print('Error in indexing data')
            print(str(ex))
            is_stored = False
        finally:
            return is_stored
    
    def search(self, index_name, search):
        res = self._es.search(index=index_name, body=search)
        # print(res)
        return res