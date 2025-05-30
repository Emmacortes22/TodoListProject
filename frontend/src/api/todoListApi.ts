import axios, { type AxiosResponse } from 'axios';
import type { TodoItem } from './types';

const apiClient = axios.create({
  baseURL: 'http://localhost:5046/api/todo-list',
  withCredentials: false,
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json'
  }
});

interface AddItemRequest {
  id: number;
  title: string;
  description: string;
  category: string;
}

interface UpdateItemRequest {
  description: string;
}

interface AddProgressionRequest {
  dateTime: string;
  percent: number;
}

export default {
  getItems(): Promise<AxiosResponse<TodoItem[]>> {
    return apiClient.get('/');
  },
  getCategories(): Promise<AxiosResponse<string[]>> {
    return apiClient.get('/categories');
  },
  addItem(item: AddItemRequest): Promise<AxiosResponse<void>> {
    return apiClient.post('/', item);
  },
  updateItem(id: number, data: UpdateItemRequest): Promise<AxiosResponse<void>> {
    return apiClient.put(`/${id}`, data);
  },
  deleteItem(id: number): Promise<AxiosResponse<void>> {
    return apiClient.delete(`/${id}`);
  },
  addProgression(id: number, progression: AddProgressionRequest): Promise<AxiosResponse<void>> {
    return apiClient.post(`/${id}/progressions`, progression);
  }
};