import json
from elastic import elastic

# we connect to elasticsearch
es = elastic()

# create an index in elasticsearch, ignore status code 400 (index already exists)
es.create_index(index_name='bilfingersearch')

# load text file
with open("../textdata/exzertifikat.json", 'r') as fjson:
    jsonInput = json.load(fjson)

# store file in elasticsearch
es.store_record(index_name="bilfingersearch", doc_type="_doc",  record=jsonInput)

# test query
query_word = 'annex'
search_object = {'query': {'match': {'text': query_word }}}
search_result=es.search(index_name="bilfingersearch", search=search_object)

print("You most likely are interested in: %s%s" % (search_result['hits']['hits'][0]['_source']['folder'], search_result['hits']['hits'][0]['_source']['filename']))