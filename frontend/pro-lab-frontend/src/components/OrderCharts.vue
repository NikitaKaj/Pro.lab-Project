<script setup lang="ts">
import { Chart, registerables, type ChartConfiguration, type ChartData } from 'chart.js'
import { onMounted, onBeforeUnmount, ref, watch } from 'vue'
Chart.register(...registerables)

const props = defineProps<{ labels: string[]; series: number[] }>()
const el = ref<HTMLCanvasElement | null>(null)
let chart: Chart<'line'> | null = null

function buildData(): ChartData<'line'> {
  return {
    labels: props.labels,
    datasets: [
      {
        data: props.series,
        fill: true,
        borderColor: '#0e5fd8',
        backgroundColor: 'rgba(14,95,216,.12)',
        tension: 0.46,
        pointRadius: 0,
        borderWidth: 5,
      },
    ],
  }
}

function ensureChart() {
  if (!el.value) return

  const cfg: ChartConfiguration<'line'> = {
    type: 'line',
    data: buildData(),
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        y: {
          beginAtZero: true,
          ticks: { precision: 0 },
          grid: { color: '#e5e7eb' },
        },
        x: { grid: { color: '#eef2f7' } },
      },
      plugins: { legend: { display: false } },
    },
  }

  chart = new Chart(el.value, cfg)
}

function updateChart() {
  if (!chart) return
  chart.data.labels = props.labels
  chart.data.datasets[0].data = props.series
  chart.update()
}

onMounted(() => {
  ensureChart()
  updateChart()
})

watch(
  () => [props.labels, props.series] as const,
  () => updateChart(),
  { deep: true }
)

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
