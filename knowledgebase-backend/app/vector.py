from pymilvus import connections, CollectionSchema, FieldSchema, DataType, Collection
import hashlib
import numpy as np

connections.connect(host="milvus", port="19530")

def insert_embedding(file_id: int, text: str):
    vec = [float((int(c, 16) % 10)) for c in hashlib.md5(text.encode()).hexdigest()[:128]]
    schema = CollectionSchema([
        FieldSchema(name="id", dtype=DataType.INT64, is_primary=True, auto_id=False),
        FieldSchema(name="embedding", dtype=DataType.FLOAT_VECTOR, dim=len(vec))
    ])
    try:
        collection = Collection("files")
    except:
        collection = Collection(name="files", schema=schema)
    if not collection.has_index():
        collection.create_index(field_name="embedding", index_params={"index_type": "IVF_FLAT", "metric_type": "L2", "params": {"nlist": 128}})
    collection.insert([[file_id], [vec]])
    collection.load()
