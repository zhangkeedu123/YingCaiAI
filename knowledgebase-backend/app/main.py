from fastapi import FastAPI
from app.routes import router

app = FastAPI(title="Knowledge Base API", description="API for uploading and searching files", version="1.0.0")

app.include_router(router)
