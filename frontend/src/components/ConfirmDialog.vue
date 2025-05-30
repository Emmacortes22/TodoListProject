<template>
    <div class="modal-overlay" v-if="visible">
      <div class="modal">
        <p>{{ message }}</p>
        <div class="modal-buttons">
          <button class="btn btn-primary" @click="$emit('confirm')">Sí</button>
          <button class="btn btn-danger" @click="$emit('cancel')">No</button>
        </div>
      </div>
    </div>
  </template>
  
  <script lang="ts" setup>
  defineProps<{
    visible: boolean;
    message: string;
  }>();
  
  defineEmits(['confirm', 'cancel']);
  </script>
  
  <style scoped>
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
  }
  
  .modal {
    background: white;
    padding: 20px;
    border-radius: 8px;
    text-align: center;
    color: black;
  }
  
  .modal-buttons {
    margin-top: 15px;
    display: flex;
    justify-content: space-around;
  }

  .btn-primary {
    background-color: #4CAF50;
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 5px;
    width: 100px;
}

.btn-danger {
    background-color: #f44336;
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-right: 5px;
    width: 100px;
}
  </style>
  