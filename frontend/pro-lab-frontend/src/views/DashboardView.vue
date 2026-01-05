<script setup lang="ts">
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import TopMetricsRow from '@/components/TopMetricsRow.vue'
import OrdersChart from '@/components/OrderCharts.vue'

import { ref, onMounted, computed } from 'vue'
import { OrdersClient, OrderStatus, type GetOrderResponse } from '@/api-client/clients'

function onGenerateRoute() {
  alert('Generating optimal route…')
}

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:5001'
const ordersApi = new OrdersClient(baseUrl)

const loading = ref(false)
const error = ref<string | null>(null)
const orders = ref<GetOrderResponse[]>([])

const labels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

function startOfWeekMonday(d: Date) {
  const date = new Date(d)
  date.setHours(0, 0, 0, 0)
  const day = date.getDay()
  const diff = day === 0 ? -6 : 1 - day
  date.setDate(date.getDate() + diff)
  return date
}

function parseCreatedAt(value: string | null): Date | null {
  if (!value) return null

  const m = value.match(/^(\d{2})\.(\d{2})\.(\d{4})$/)
  if (m) {
    const dd = Number(m[1])
    const mm = Number(m[2]) - 1
    const yyyy = Number(m[3])
    const d = new Date(yyyy, mm, dd)
    if (!Number.isNaN(d.getTime())) return d
  }

  return null
}

function weekdayIndexMon0(d: Date) {
  const day = d.getDay()
  return day === 0 ? 6 : day - 1
}

const series = computed(() => {
  const now = new Date()
  const start = startOfWeekMonday(now)
  const end = new Date(start)
  end.setDate(end.getDate() + 7)

  const buckets = Array(7).fill(0)

  for (const o of orders.value) {
    const dt = parseCreatedAt(o.createdAt)
    if (!dt) continue
    if (dt < start || dt >= end) continue

    const idx = weekdayIndexMon0(dt)
    buckets[idx]++
  }

  return buckets as number[]
})

const completed = computed(() => orders.value.filter(o => o.status === OrderStatus.Completed).length)
const inProgress = computed(() => orders.value.filter(o => o.status === OrderStatus.InRoute).length)
const canceled = computed(() => orders.value.filter(o => o.status === OrderStatus.Cancelled).length)
const ordersToday = computed(() => {
  const today = new Date()
  today.setHours(0,0,0,0)
  const tomorrow = new Date(today)
  tomorrow.setDate(tomorrow.getDate() + 1)

  return orders.value.filter(o => {
    const dt = parseCreatedAt(o.createdAt)
    return dt && dt >= today && dt < tomorrow
  }).length
})

async function loadOrders() {
  loading.value = true
  error.value = null
  try {
    orders.value = await ordersApi.get()
  } catch (e: any) {
    error.value = e?.message ?? 'Failed to load orders'
    orders.value = []
  } finally {
    loading.value = false
  }
}

onMounted(loadOrders)
</script>

<template>
  <div class="flex min-h-screen bg-white text-gray-900">
    <Sidebar />

    <main class="flex-1 overflow-auto">
      <div class="w-full px-12 py-12">
        <PageHeader title="Dashboard" @action="onGenerateRoute" />

        <div class="mt-4 text-sm text-gray-600">
          <span v-if="loading">Loading…</span>
          <span v-else-if="error" class="text-red-600">{{ error }}</span>
          <span v-else>&nbsp;</span>
        </div>

        <TopMetricsRow
          :items="[
            { label: 'Orders Today', value: ordersToday },
            { label: 'Completed', value: completed },
            { label: 'In Progress', value: inProgress },
            { label: 'Canceled', value: canceled },
          ]"
        />

        <div class="mt-8 grid gap-6 xl:grid-cols-[300px,1fr,360px] 2xl:grid-cols-[320px,1fr,380px] !mt-10">
          <OrdersChart
            class="rounded-lg border border-gray-200 shadow-sm"
            :labels="labels"
            :series="series"
          />
        </div>
      </div>
    </main>
  </div>
</template>
