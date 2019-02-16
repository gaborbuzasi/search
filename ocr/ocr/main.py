from elastic import elastic
# from logstash import logstash
from load_data_into_json import (find_files, start_the_data_input_process)
import json

ROOT = '/mnt/data/classified_documents'

if __name__ == "__main__":

    es = elastic()
    # create an index in elasticsearch, ignore status code 400 (index already exists)
    es.create_index(index_name='bilfingersearch')

    for fp in find_files(ROOT, '.*'):
        msg = start_the_data_input_process(input_folder)

        es.store_record(
            index_name="bilfingersearch", doc_type="_doc", record=jsonInput)
