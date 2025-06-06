<template>
    <ErrorBanner v-if="errorMessage" :message="errorMessage" @close="clearError" />
    <div class="container">
        <h1 class="title">Aplicación Todo List</h1>
        <div class="cards-wrapper">
            <div class="card add-task-card">
                <h2 class="card-title">Agregar Nueva Tarea</h2>
                <form @submit.prevent="addItem">
                    <div class="form-group">
                        <label>Título:</label>
                        <input v-model="newItem.title" required>
                    </div>
                    <div class="form-group">
                        <label>Descripción:</label>
                        <textarea v-model="newItem.description" required></textarea>
                    </div>
                    <div class="form-group">
                        <label>Categoría:</label>
                        <select v-model="newItem.category" required>
                            <option v-for="category in categories" :key="category" :value="category">
                                {{ category }}
                            </option>
                        </select>
                    </div>
                    <button type="submit" class="btn btn-primary">Agregar Tarea</button>
                </form>
            </div>

            <div class="card task-list-card">
                <h2 class="card-title">Lista de Tareas</h2>
                <div v-if="loading" class="loading">Cargando...</div>
                <div v-else>
                    <div v-if="items.length === 0" class="empty-state">
                        <p class="empty-state-text">No hay tareas</p>
                    </div>
                    <div v-for="item in items" :key="item.id" class="todo-item">
                        <h3>{{ item.title }} - {{ item.description }} [{{ item.category }}]</h3>
                        <p>Completado: {{ item.isCompleted ? 'Si' : 'No' }}</p>
                        
                        <div class="progress-container">
                            <div class="progress-bar" :style="{ width: totalProgress(item) + '%' }"></div>
                            <span class="progress-text">{{ totalProgress(item) }}%</span>
                        </div>
                        
                        <div class="progressions">
                            <div v-for="(prog, index) in item.progressions" :key="index" class="progression">
                            <div class="progression-header">
                                {{ formatDateTime(prog.date) }} - {{ prog.percent }}% (Total Acumulado: {{ accumulatedPercent(item, index) }}%)
                            </div>
                            <div class="progress-bar-container">
                                <div class="progress-bar" :style="{ width: accumulatedPercent(item, index) + '%' }"></div>
                            </div>
                            </div>
                        </div>
                        
                        <div class="actions">
                            <button @click="showUpdateForm(item)" :disabled="item.isCompleted" class="btn btn-secondary">Actualizar</button>
                            <button @click="requestDelete(item.id)" class="btn btn-danger">Eliminar</button>
                            <button @click="showAddProgression(item.id)" class="btn btn-primary">Agregar Progreso</button>
                        </div>
                        
                        <div v-if="itemToUpdate === item.id" class="edit-form">
                            <textarea v-model="updatedDescription"></textarea>
                            <button @click="updateItem(item.id)" class="btn btn-primary">Guardar</button>
                            <button @click="cancelUpdate" class="btn btn-secondary">Cancelar</button>
                        </div>
                        
                        <div v-if="itemToAddProgression === item.id" class="progression-form">
                            <input type="date" v-model="newProgression.date" required>
                            <input type="number" v-model="newProgression.percent" min="1" max="100" required>
                            <button @click="addProgression(item.id)" class="btn btn-primary">Agregar</button>
                            <button @click="cancelAddProgression" class="btn btn-secondary">Cancelar</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
    </div>
    <ConfirmDialog
        :visible="showConfirm"
        message="¿Estás seguro de que quieres eliminar esta tarea?"
        @confirm="confirmDelete"
        @cancel="cancelDelete"
    />
</template>
  
<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import type { TodoItem, Progression } from '../api/types';
import todoListApi from '../api/todoListApi';
import ErrorBanner from './ErrorBanner.vue';
import ConfirmDialog from './ConfirmDialog.vue';

const items = ref<TodoItem[]>([]);
const loading = ref<boolean>(true);
const categories = ref<string[]>([]);
const newItem = ref<TodoItem>({
    id: 0,
    title: '',
    description: '',
    category: 'Trabajo',
    isCompleted: false,
    progressions: []
});
const itemToUpdate = ref<number | null>(null);
const updatedDescription = ref<string>('');
const itemToAddProgression = ref<number | null>(null);
const newProgression = ref<Progression>({
    date: new Date().toISOString().slice(0, 10),
    percent: 10
});
const errorMessage = ref<string>('');
const showConfirm = ref(false);
const idToDelete = ref<number | null>(null);

onMounted(async (): Promise<void> => {
    await fetchItems();
    await fetchCategories();
});

async function fetchItems(): Promise<void> {
    try {
        loading.value = true;
        const response = await todoListApi.getItems();
        items.value = response.data;
    } catch (error) {
        showError('Error al cargar las tareas');
    } finally {
        loading.value = false;
    }
}

async function fetchCategories(): Promise<void> {
  try {
    const response = await todoListApi.getCategories();
    categories.value = response.data;
  } catch (error) {
    showError("Error al cargar las categorías");
  }
}

async function addItem(): Promise<void> {
    const title = newItem.value.title.trim();
    const description = newItem.value.description.trim();

    if (!title || !description) {
        showError('Todos los campos son obligatorios y no pueden estar vacíos.');
        return;
    }
    
    try {
        await todoListApi.addItem({
            id: 0,
            title: title,
            description: description,
            category: newItem.value.category,
        });

        newItem.value.title = '';
        newItem.value.description = '';
        newItem.value.category = 'Trabajo';
        
        await fetchItems();
    } catch (error) {
        showError((error as any).response?.data || (error as Error).message);
    }
}

function showUpdateForm(item: TodoItem): void {
    itemToUpdate.value = item.id;
    updatedDescription.value = item.description;
}

function cancelUpdate(): void {
    itemToUpdate.value = null;
}

async function updateItem(id: number): Promise<void> {
    try {
        await todoListApi.updateItem(id, { description: updatedDescription.value });
        itemToUpdate.value = null;
        await fetchItems();
    } catch (error) {
        showError((error as any).response?.data || (error as Error).message);
    }
}

async function confirmDelete(): Promise<void> {
  if (idToDelete.value !== null) {
    try {
      await todoListApi.deleteItem(idToDelete.value);
      await fetchItems();
    } catch (error) {
      showError((error as any).response?.data || (error as Error).message);
    } finally {
      idToDelete.value = null;
      showConfirm.value = false;
    }
  }
}

function showAddProgression(id: number): void {
    itemToAddProgression.value = id;
    newProgression.value = {
        date: new Date().toISOString().slice(0, 10),
        percent: 10
    };
}

function cancelAddProgression(): void {
    itemToAddProgression.value = null;
}

async function addProgression(id: number): Promise<void> {
    try {
        await todoListApi.addProgression(id, {
        dateTime: newProgression.value.date,
        percent: newProgression.value.percent
        });
        itemToAddProgression.value = null;
        await fetchItems();
    } catch (error) {
        showError((error as any).response?.data || (error as Error).message);
    }
}

function totalProgress(item: TodoItem): number {
    return item.progressions.reduce((total: number, prog: Progression) => total + prog.percent, 0);
}

function accumulatedPercent(item: TodoItem, index: number): number {
    return item.progressions.slice(0, index + 1).reduce((sum: number, p: Progression) => sum + p.percent, 0);
}

function formatDateTime(dateString: string): string {
  const date = new Date(dateString);
  
  return date.toLocaleString('en-US', {
    month: '2-digit',
    day: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: true
  });
}

function showError(message: string) {
  errorMessage.value = message;
}

function clearError() {
  errorMessage.value = '';
}

function requestDelete(id: number): void {
  idToDelete.value = id;
  showConfirm.value = true;
}

function cancelDelete(): void {
  showConfirm.value = false;
  idToDelete.value = null;
}
</script>
  
<style scoped>
.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
}

.title {
    color: #fff;
    text-align: center;
}

.cards-wrapper {
  display: flex;
  gap: 20px;
  width: 100%;
  justify-content: center;
  align-items: flex-start;
}

.card {
  background: #394f4e;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  padding: 20px;
  width: 400px;
}

.add-task-card {
  position: sticky;
  top: 20px;
  height: fit-content;
}

.task-list-card {
  flex-grow: 1;
}

.card-title {
    color: #fff;
    text-align: center;
}

.empty-state {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100%;
}


.form-group {
    margin-bottom: 15px;
}

.form-group label {
    display: block;
    margin-bottom: 5px;
    font-weight: bold;
}

.form-group input,
.form-group textarea,
.form-group select {
    width: 100%;
    padding: 8px;
    border: 1px solid #ddd;
    border-radius: 4px;
    box-sizing: border-box;
}

.form-group textarea {
    min-height: 80px;
}

.btn-primary {
    background-color: #4CAF50;
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 5px;
}

.btn-secondary {
    background-color: #2196F3;
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 5px;
}

.btn-danger {
    background-color: #f44336;
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 5px;
}

.todo-item {
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 15px;
    margin-bottom: 15px;
    display: flex;
    flex-direction: column;
}

.progress-container {
    height: 20px;
    background-color: #e0e0e0;
    border-radius: 10px;
    margin: 10px 0;
    position: relative;
}

.progress-bar {
    height: 100%;
    background-color: #4CAF50;
    border-radius: 10px;
    transition: width 0.3s;
}

.progress-text {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    color: #333;
    font-size: 12px;
}

.progressions {
    margin: 10px 0;
}

.progression {
    background-color: #f5f5f5;
    padding: 5px 10px;
    border-radius: 4px;
    margin-bottom: 5px;
    font-size: 14px;
    color: #333;
    align-content: center;
}

.actions {
    display: flex;
    justify-content: center;
    gap: 10px;
    margin-top: 1px;
}

.btn {
    transition: all 0.3s ease;
}

.btn:hover {
     transform: scale(1.05);
}

.edit-form,
.progression-form {
    margin-top: 15px;
    padding: 15px;
    background-color: #f9f9f9;
    border-radius: 4px;
}

.edit-form textarea {
    width: 100%;
    min-height: 80px;
    margin-bottom: 10px;
    border: 1px solid #ddd;
    border-radius: 4px;
    box-sizing: border-box;
}

.progression-form input {
    display: block;
    width: 95%;
    margin-bottom: 10px;
    padding: 8px;
    border: 1px solid #ddd;
    border-radius: 4px;
}

.loading {
    text-align: center;
    padding: 20px;
    color: #666;
}
</style>