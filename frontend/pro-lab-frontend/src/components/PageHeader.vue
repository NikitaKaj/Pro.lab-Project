<script setup lang="ts">
defineProps<{ title: string }>()
defineEmits<{ (e: 'action'): void }>()
</script>

<template>
  <div class="flex items-center justify-between pb-10">
    <h1 class="text-4xl font-extrabold">{{ title }}</h1>
    <button
      class="bg-[#1673ea] hover:bg-[#105fc6] text-white font-semibold px-7 py-3.5 rounded-md shadow"
      @click="$emit('action')"
    >
      Generate Optimal Route
    </button>
  </div>
</template>


