<template>
    <div v-if="visible" class="error-banner">
      <span>{{ message }}</span>
      <button class="close-btn" @click="close">✖</button>
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, watch } from 'vue';
  
  const props = defineProps<{
    message: string
  }>();
  
  const emit = defineEmits(['close']);
  
  const visible = ref(true);
  
  watch(() => props.message, () => {
    visible.value = true;
  });
  
  function close() {
    visible.value = false;
    emit('close');
  }
  </script>
  
  <style scoped>
  .error-banner {
    background-color: #ffcccc;
    color: #a94442;
    padding: 10px 15px;
    border: 1px solid #ebccd1;
    border-radius: 4px;
    margin-bottom: 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-weight: bold;
  }
  .close-btn {
    background: none;
    border: none;
    font-size: 16px;
    cursor: pointer;
    color: #a94442;
  }
  </style>
  