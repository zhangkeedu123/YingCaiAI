# Knowledge Base Backend

## 功能
- 用户上传文本文件，内容写入 PostgreSQL 和向量写入 Milvus
- 支持 Swagger API 调试页面（FastAPI）

## 使用方式

```bash
git clone https://github.com/yourname/knowledgebase-backend.git
cd knowledgebase-backend
sudo docker-compose up -d --build
```

访问：http://localhost:8000/docs 查看 Swagger 页面
