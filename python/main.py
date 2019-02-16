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
query_phrase = 'Scelta i una'

# mlt_query = {
#                 'query': {
#                     'more_like_this': {
#                         'fields': ['folder', 'filename', 'text'],
#                         'like': query_word
#                     }
#                 }
#             }
response = es.search(index_name="bilfingersearch",  query_phrase=query_phrase)

best_result = es.get_best_result(response)

print("You most likely are interested in: %s" % (best_result))