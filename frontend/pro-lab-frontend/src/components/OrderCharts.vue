<script setup lang="ts">
import { Chart, registerables, type ChartConfiguration, type ChartData } from 'chart.js'
import { onMounted, onBeforeUnmount, ref } from 'vue'
Chart.register(...registerables)

const props = defineProps<{ labels: string[]; series: number[] }>()
const el = ref<HTMLCanvasElement | null>(null)
let chart: Chart | null = null

onMounted(() => {
  if (!el.value) return
  const data: ChartData<'line'> = {
    labels: props.labels,
    datasets: [{
      data: props.series,
      fill: true,
      borderColor: '#0e5fd8',
      backgroundColor: 'rgba(14,95,216,.12)',
      tension: 0.46,
      pointRadius: 0,
      borderWidth: 5,
    }],
  }
  const cfg: ChartConfiguration<'line'> = {
    type: 'line',
    data,
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: { beginAtZero: true, grid: { color: '#e5e7eb' } },
        x: { grid: { color: '#eef2f7' } },
      },
      plugins: { legend: { display: false } },
    },
  }
  chart = new Chart(el.value, cfg)
})

onBeforeUnmount(() => chart?.destroy())
</script>

<template>
  <section :class="['overflow-hidden', $attrs.class]">
    <div class="px-6 py-4 text-gray-700 text-base">Orders</div>
    <div class="px-6 pb-6">
      <div class="h-[400px]">
        <canvas ref="el" class="w-full h-full"></canvas>
      </div>
    </div>
  </section>
</template>


