from pydantic import BaseModel

class FileMetadata(BaseModel):
    filename: str
    content: str
