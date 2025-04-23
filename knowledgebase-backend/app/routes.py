from fastapi import APIRouter, UploadFile, File
from app import database, vector, models
from app.schemas import FileMetadata

router = APIRouter()

@router.post("/upload")
async def upload_file(file: UploadFile = File(...)):
    content = await file.read()
    text = content.decode("utf-8")
    
    file_id = database.save_file_metadata(file.filename, text)
    vector.insert_embedding(file_id, text)
    
    return {"status": "ok", "file_id": file_id}
