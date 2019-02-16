from elasticsearch import Elasticsearch
from operator import itemgetter

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
                            "type": "text",
                            "analyzer": "german",
                            "fields": {
                                "raw": {
                                    "type":"keyword"
                                }
                            }
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

    def delete_index(self, index_name):
        self._es.indices.delete(index=index_name, ignore=[400, 404])
    
    def store_record(self, index_name, doc_type, record):
        is_stored = True
        try:
            item = self._es.get(index=index_name, doc_type='_doc', id=record['filename'], ignore=[404])
            if item['found'] == True:
                print('Item with id %s already stored' % (record['filename']))
                return True
            outcome = self._es.index(index=index_name, doc_type=doc_type, body=record, id=record['filename'])
            # print(outcome)
        except Exception as ex:
            print('Error in indexing data')
            print(str(ex))
            is_stored = False
        finally:
            return is_stored
    
    def search(self, index_name, search):
        response = self._es.search(index=index_name, body=search)
        return response
    
    def get_best_result(self, response):
        filenames = {}
        for hit in response['hits']['hits']:
            score = hit['_score']
            filename = hit['_source']['filename']
            if filename not in filenames:
                filenames[filename] = score
            else:
                filenames[filename] += score
        if len(filenames) > 0:
            filenames_sorted_by_score = sorted(filenames.items(),key=itemgetter(1), reverse=True)
            filename = filenames_sorted_by_score[0][0]
            return filename

