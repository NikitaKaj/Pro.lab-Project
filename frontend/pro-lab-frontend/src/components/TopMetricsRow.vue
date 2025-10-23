<template>
  <div class="mt-8 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
    <MetricCard v-for="(it, i) in items" :key="i" :label="it.label" :value="it.value" size="lg" />
  </div>
</template>

<script setup lang="ts">
import MetricCard from './MetricCard.vue'
defineProps<{ items: Array<{ label: string; value: string|number }> }>()
</script>
