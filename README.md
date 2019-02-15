# search

to deploy the docker cluster:

```
$ ELK_VERSION=6.6.0 docker-compose up -d
```

to test ingestion by logstash:

```
curl -X POST 'http://localhost:5000/' -H "Content-Type: text/plain" -d '@./tests/logstash-tutorial.log'
```

to query elastic search:

```
curl 'http://localhost:9200/_search?pretty'
```
