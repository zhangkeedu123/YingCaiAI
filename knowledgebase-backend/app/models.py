from sqlalchemy import Column, Integer, String, Text
from app.database import Base

class File(Base):
    __tablename__ = "files"
    id = Column(Integer, primary_key=True, index=True)
    filename = Column(String, unique=True, index=True)
    content = Column(Text)
