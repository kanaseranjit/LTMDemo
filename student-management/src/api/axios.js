import axious from 'axios';

const api=axious.create({
    baseURL:'https://localhost:7093/api',
    headers:{
        'Content-Type':'application/json'
    }
});

export default api;