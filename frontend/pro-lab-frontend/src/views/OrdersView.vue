<script setup lang="ts">
import { Bars3Icon } from '@heroicons/vue/24/outline'
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import { ref, computed, onMounted } from 'vue'


const sidebarOpen = ref(false)
import {
  OrdersClient,
  OrderStatus,
  type GetOrderResponse,
  type PlaceOrderRequest,
  CouriersClient,
  type CourierResponse,
} from '@/api-client/clients'

type OrderRow = {
  id: number
  created: string
  customer: string
  address: string
  status: OrderStatus
  courierId?: number
  courierName?: string
}

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:5001'
const ordersApi = new OrdersClient(baseUrl)
const couriersApi = new CouriersClient(baseUrl)

const items = ref<OrderRow[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

const couriers = ref<CourierResponse[]>([])
const couriersLoading = ref(false)
const couriersError = ref<string | null>(null)

const submitting = ref(false)

const currentPage = ref(1)
const perPage = 10

const totalPages = computed(() => Math.ceil(items.value.length / perPage))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * perPage
  return items.value.slice(start, start + perPage)
})

function nextPage() {
  if (currentPage.value < totalPages.value) currentPage.value++
}

function prevPage() {
  if (currentPage.value > 1) currentPage.value--
}

function formatDateRu(value: string | null) {
  if (!value) return '-'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return value
  return d.toLocaleDateString('ru-RU')
}

function toRow(o: GetOrderResponse): OrderRow {
  return {
    id: o.orderId,
    created: formatDateRu(o.createdAt),
    customer: o.customer ?? '-',
    address: o.address ?? '-',
    status: o.status,
  }
}

async function loadOrders() {
  loading.value = true
  error.value = null
  try {
    const data = await ordersApi.get()
    items.value = (data ?? []).map(toRow)
    if (currentPage.value > totalPages.value) currentPage.value = Math.max(1, totalPages.value)
  } catch (e: any) {
    error.value = e?.message ?? 'Failed to load orders'
  } finally {
    loading.value = false
  }
}

async function loadCouriers() {
  couriersLoading.value = true
  couriersError.value = null
  try {
    const data = await couriersApi.get()
    couriers.value = data ?? []
  } catch (e: any) {
    couriersError.value = e?.message ?? 'Failed to load couriers'
  } finally {
    couriersLoading.value = false
  }
}

async function deleteOrder(id: number) {
  try {
    await ordersApi.delete(id)
    items.value = items.value.filter(x => x.id !== id)
    if (currentPage.value > totalPages.value) currentPage.value = Math.max(1, totalPages.value)
  } catch (e: any) {
    alert(e?.message ?? 'Delete failed')
  }
}

async function setStatus(id: number, status: OrderStatus) {
  try {
    await ordersApi.update(id, status)
    const row = items.value.find(x => x.id === id)
    if (row) row.status = status
  } catch (e: any) {
    alert(e?.message ?? 'Update status failed')
  }
}

function statusLabel(s: OrderStatus) {
  switch (s) {
    case OrderStatus.Pending: return 'Pending'
    case OrderStatus.InRoute: return 'InRoute'
    case OrderStatus.Completed: return 'Completed'
    case OrderStatus.Cancelled: return 'Cancelled'
    default: return String(s)
  }
}

function courierLabel(c: CourierResponse) {
  return `${c.fullName} (#${c.courierId})`
}

const showAddModal = ref(false)

const newOrder = ref({
  customer: '',
  address: '',
  status: OrderStatus.Pending as OrderStatus,
  courierId: null as number | null,
})

function openAddModal() {
  if (!couriersLoading.value && couriers.value.length === 0 && !couriersError.value) {
    loadCouriers()
  }

  newOrder.value = {
    customer: '',
    address: '',
    status: OrderStatus.Pending,
    courierId: couriers.value[0]?.courierId ?? null,
  }
  showAddModal.value = true
}

async function addOrder() {
  const customer = newOrder.value.customer.trim()
  const address = newOrder.value.address.trim()
  const courierId = newOrder.value.courierId

  if (!customer || !address) return
  if (courierId == null) return

  submitting.value = true
  try {
    const req: PlaceOrderRequest = {
      customer,
      address,
      status: newOrder.value.status,
      courierId,
    }

    await ordersApi.place(req)

    showAddModal.value = false
    currentPage.value = 1

    await loadOrders()
  } catch (e: any) {
    alert(e?.message ?? 'Create order failed')
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadOrders(), loadCouriers()])
})</script>

<template>
  <div class="min-h-screen bg-white text-gray-900 md:flex">
    <Sidebar v-model:open="sidebarOpen" />
<div class="flex-1 px-4 py-6 sm:px-6 sm:py-8 lg:px-12 lg:py-12">
      <div class="flex items-center gap-3">
          <button
            class="md:hidden p-2 rounded-md border border-gray-200 bg-white"
            @click="sidebarOpen = true"
            aria-label="Open menu"
          >
            <Bars3Icon class="w-6 h-6" />
          </button>

          <div class="flex-1">
            <PageHeader title="Orders" />
          </div>
        </div>

      <div class="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:pr-[80px]">
        <div class="text-sm text-gray-600">
          <span v-if="loading">Loading…</span>
          <span v-else-if="error" class="text-red-600">{{ error }}</span>
          <span v-else>&nbsp;</span>
        </div>

        <div class="flex gap-3">
          <button
            class="border border-[#1673ea] text-[#1673ea] font-semibold px-5 py-2.5 rounded-md shadow bg-white hover:bg-[#1673ea] hover:text-white transition"
            @click="openAddModal"
          >
            Add Order
          </button>

          <button
            class="border border-gray-300 text-gray-700 font-semibold px-5 py-2.5 rounded-md shadow bg-white hover:bg-gray-100 transition"
            @click="loadOrders"
          >
            Refresh
          </button>
        </div>
      </div>

      <div class="flex justify-center !mt-5">
        <table class="table-fixed border-2 w-[90%] bg-white">
          <thead class="border-2 font-bold">
            <tr>
              <th class="border p-3 w-[10%]">ID</th>
              <th class="border w-[15%]">Created</th>
              <th class="border w-[18%]">Customer</th>
              <th class="border w-[25%]">Address</th>
              <th class="border w-[12%]">Status</th>
              <th class="border w-[10%]">Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr v-if="!loading && paginatedItems.length === 0">
              <td class="border p-6 text-center text-gray-500" colspan="7">
                No orders yet
              </td>
            </tr>

            <tr
              v-else
              v-for="order in paginatedItems"
              :key="order.id"
              class="text-center border"
            >
              <td class="border p-3">#{{ order.id }}</td>
              <td class="border">{{ order.created }}</td>
              <td class="border">{{ order.customer }}</td>
              <td class="border">{{ order.address }}</td>

              <td class="border">
                <select
                  class="border rounded px-2 py-1 bg-white"
                  :value="order.status"
                  @change="setStatus(order.id, Number(($event.target as HTMLSelectElement).value) as any)"
                >
                  <option :value="OrderStatus.Pending">{{ statusLabel(OrderStatus.Pending) }}</option>
                  <option :value="OrderStatus.InRoute">{{ statusLabel(OrderStatus.InRoute) }}</option>
                  <option :value="OrderStatus.Completed">{{ statusLabel(OrderStatus.Completed) }}</option>
                  <option :value="OrderStatus.Cancelled">{{ statusLabel(OrderStatus.Cancelled) }}</option>
                </select>
              </td>

              <td class="border p-3">
                <div class="flex items-center justify-center gap-6 cursor-pointer">
                  <img
                    src="@/assets/images/DeleteUser.png"
                    class="w-6"
                    @click="deleteOrder(order.id)"
                    title="Delete"
                  />
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="flex justify-end pr-[80px] pt-[14px] items-center">
        <span style="margin-right: 14px;">
          {{ items.length === 0 ? 0 : (currentPage - 1) * perPage + 1 }} -
          {{ Math.min(currentPage * perPage, items.length) }} of
          {{ items.length }}
        </span>

        <button
          @click="prevPage"
          :disabled="currentPage === 1"
          class="px-3 py-1 border rounded disabled:opacity-40"
        >
          ‹
        </button>

        <span class="mx-2 px-3 py-1 border rounded bg-gray-100">
          {{ currentPage }}
        </span>

        <button
          @click="nextPage"
          :disabled="currentPage === totalPages || totalPages === 0"
          class="px-3 py-1 border rounded disabled:opacity-40"
        >
          ›
        </button>
      </div>
    </div>

    <div v-if="showAddModal" class="fixed inset-0 bg-black/30 flex justify-center items-center z-50">
      <div class="bg-white p-6 rounded-lg shadow-lg w-[420px]">
        <h2 class="text-xl font-bold mb-4 text-center !mb-2">Add New Order</h2>

        <div class="text-sm text-gray-600 mb-3">
          <span v-if="couriersLoading">Loading couriers…</span>
          <span v-else-if="couriersError" class="text-red-600">{{ couriersError }}</span>
          <span v-else>&nbsp;</span>
        </div>

        <select
          v-model="newOrder.courierId"
          class="w-full border p-2 rounded mb-3 bg-white"
          :disabled="couriersLoading || couriers.length === 0 || submitting"
          style="margin-bottom: 10px;"
        >
          <option v-if="couriers.length === 0" :value="null">No couriers</option>
          <option v-for="c in couriers" :key="c.courierId" :value="c.courierId">
            {{ courierLabel(c) }}
          </option>
        </select>

        <input
          v-model="newOrder.customer"
          type="text"
          placeholder="Customer"
          class="w-full border p-2 rounded mb-3"
          :disabled="submitting"
          style="margin-bottom: 10px;"
        />

        <input
          v-model="newOrder.address"
          type="text"
          placeholder="Address"
          class="w-full border p-2 rounded mb-3"
          :disabled="submitting"
          style="margin-bottom: 10px;"
        />

        <select
          v-model="newOrder.status"
          class="w-full border p-2 rounded mb-4 bg-white"
          :disabled="submitting"
        >
          <option :value="OrderStatus.Pending">Pending</option>
          <option :value="OrderStatus.InRoute">InRoute</option>
          <option :value="OrderStatus.Completed">Completed</option>
          <option :value="OrderStatus.Cancelled">Cancelled</option>
        </select>

        <div class="flex justify-between !mt-5">
          <button
            @click="showAddModal = false"
            :disabled="submitting"
            class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400 disabled:opacity-50"
          >
            Cancel
          </button>

          <button
            @click="addOrder"
            :disabled="submitting || couriersLoading || couriers.length === 0 || newOrder.courierId == null"
            class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6] disabled:opacity-50"
          >
            {{ submitting ? 'Adding…' : 'Add' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
