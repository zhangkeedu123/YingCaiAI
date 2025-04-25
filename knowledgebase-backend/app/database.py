from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

DB_URL = "postgresql://postgres:123456@postgres:5432/knowledgebase"
engine = create_engine(DB_URL)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()

def save_file_metadata(filename: str, content: str) -> int:
    from app.models import File
    db = SessionLocal()
    Base.metadata.create_all(bind=engine)
    new_file = File(filename=filename, content=content)
    db.add(new_file)
    db.commit()
    db.refresh(new_file)
    db.close()
    return new_file.id
