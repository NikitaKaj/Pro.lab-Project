<template>
  <div class="space-y-4">
    <MetricCard
      v-for="(c, i) in cards"
      :key="i"
      :label="c.label"
      :value="c.value"
      :suffix="c.suffix"
      size="sm"
    />
  </div>
</template>

<script setup lang="ts">
import MetricCard from './MetricCard.vue'

defineProps<{
  cards: Array<{ label: string; value: string | number; suffix?: string }>
}>()
</script>
