<script setup lang="ts">
defineProps<{ title: string }>()
defineEmits<{ (e: 'action'): void }>()
</script>

<template>
  <div class="flex items-center justify-between pb-10">
    <h1 class="text-4xl font-extrabold">{{ title }}</h1>
  </div>
</template>


