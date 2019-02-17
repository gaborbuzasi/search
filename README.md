# search

to deploy the docker cluster:

```
$ mkdir -p /var/log/ocr && DATA_SOURCE=/path/to/source/data ELK_VERSION=6.6.0 docker-compose up -d
```

to test ingestion by logstash:

```
curl -X POST 'http://localhost:5000/' -H "Content-Type: text/plain" -d '@./tests/logstash-tutorial.log'
```

to query elastic search:

```
curl 'http://localhost:9200/_search?pretty'
```

to recrate the environment from scratch:

```
$ docker rm -f ocr logstash elasticsearch && docker rmi -f search_ocr && rm -rf data/
```

## WARNING!

OCR processing is automatically launched during containers creation and takes several time!

Consider to:
 * Scale the OCR contanier (use more than one)
 * Allocate as more resources as possible to the containers
