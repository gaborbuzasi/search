from elastic import elastic
from load_data_into_json import (find_files, start_the_data_input_process)
import json

# use this for async ingestion through logstash
# from logstash import logstash

ROOT = '/mnt/data/classified_documents'

if __name__ == "__main__":

    es = elastic()
    # create an index in elasticsearch, ignore status code 400 (index already exists)
    es.create_index(index_name='bilfingersearch')

    # use this to adopt logstash ingestion
    # ls = logstash()

    for fp in find_files(ROOT):
        msg = start_the_data_input_process(fp)

        # use this to adopt logstash ingestion
        # ls.store_record(msg)

        es.store_record(
            index_name="bilfingersearch", doc_type="_doc", record=msg)

    # in case of logstash ingestion, close the stream connection
    # ls.close_connection()
