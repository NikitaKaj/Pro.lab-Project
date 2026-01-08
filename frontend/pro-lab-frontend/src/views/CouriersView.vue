<script setup lang="ts">
import { Bars3Icon } from '@heroicons/vue/24/outline'
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import { ref, computed, onMounted } from 'vue'

const sidebarOpen = ref(false)

import {
  CouriersClient,
  type CourierResponse,
  type CreateCourierRequest,
  type UpdateCourierRequest,
} from '@/api-client/clients'

type CourierRow = {
  id: number
  created: string
  courier: string
  completed: number
  active: number
}

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://prolab.azurewebsites.net'
const couriersApi = new CouriersClient(baseUrl)

const items = ref<CourierRow[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

const uiAlert = ref<{ type: 'error' | 'warning' | 'success', text: string } | null>(null)

function showAlert(type: 'error' | 'warning' | 'success', text: string) {
  uiAlert.value = { type, text }

  window.setTimeout(() => {
    if (uiAlert.value?.text === text) uiAlert.value = null
  }, 5000)
}

function extractApiErrorMessage(e: any) {
  return (
    e?.response?.data?.message ||
    e?.response?.data?.title ||
    e?.response?.data ||
    e?.message ||
    'Unexpected error'
  )
}

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

function formatDateRu(value: Date | string | null | undefined) {
  if (!value) return '-'
  const d = value instanceof Date ? value : new Date(value)
  if (Number.isNaN(d.getTime())) return String(value)
  return d.toLocaleDateString('ru-RU')
}

function toRow(c: CourierResponse): CourierRow {
  return {
    id: c.courierId,
    created: formatDateRu(c.createdAt),
    courier: c.fullName,
    completed: c.completedOrdersCount ?? 0,
    active: c.activeOrdersCount ?? 0,
  }
}

async function loadCouriers() {
  loading.value = true
  error.value = null

  try {
    const data = await couriersApi.get()
    items.value = (data ?? []).map(toRow)

    if (currentPage.value > totalPages.value) {
      currentPage.value = Math.max(1, totalPages.value)
    }
  } catch (e: any) {
    error.value = e?.message ?? 'Failed to load couriers'
  } finally {
    loading.value = false
  }
}

async function deleteCourier(id: number) {
  uiAlert.value = null

  try {
    await couriersApi.delete(id)
    items.value = items.value.filter(x => x.id !== id)

    if (currentPage.value > totalPages.value) {
      currentPage.value = Math.max(1, totalPages.value)
    }

    showAlert('success', 'Courier deleted')
  } catch (e: any) {
    const msg = extractApiErrorMessage(e)
    const status = e?.status ?? e?.response?.status

    const looksLikeHasOrders =
      status === 409 ||
      status === 400 ||
      String(msg).toLowerCase().includes('order') ||
      String(msg).toLowerCase().includes('foreign') ||
      String(msg).toLowerCase().includes('constraint')

    if (!looksLikeHasOrders) {
      showAlert('error', `Courier have active or completed orders and cannot be deleted`)
    }
  }
}

const showAddModal = ref(false)
const showEditModal = ref(false)
const editId = ref<number | null>(null)

const form = ref({
  courier: '',
})

function openAddModal() {
  form.value = { courier: '' }
  showAddModal.value = true
}

async function addCourier() {
  const name = form.value.courier.trim()
  if (!name) return

  try {
    const req: CreateCourierRequest = { fullName: name }
    await couriersApi.create(req)

    showAddModal.value = false
    await loadCouriers()
    showAlert('success', 'Courier added')
  } catch (e: any) {
    const msg = extractApiErrorMessage(e)
    showAlert('error', `Create failed: ${msg}`)
  }
}

function openEditModal(row: CourierRow) {
  editId.value = row.id
  form.value = { courier: row.courier }
  showEditModal.value = true
}

async function saveEditedCourier() {
  if (editId.value == null) return

  const name = form.value.courier.trim()
  if (!name) return

  try {
    const req: UpdateCourierRequest = { id: editId.value, fullName: name }
    await couriersApi.update(req)

    showEditModal.value = false
    editId.value = null
    await loadCouriers()
    showAlert('success', 'Courier updated')
  } catch (e: any) {
    const msg = extractApiErrorMessage(e)
    showAlert('error', `Update failed: ${msg}`)
  }
}

onMounted(loadCouriers)
</script>

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
          <PageHeader title="Couriers" />
        </div>
      </div>

      <div class="mt-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between sm:pr-[80px]">
        <div class="text-sm text-gray-600">
          <span v-if="loading">Loading…</span>
          <span v-else-if="error" class="text-red-600">{{ error }}</span>
          <span v-else>&nbsp;</span>

          <div v-if="uiAlert" class="mt-2">
            <div
              class="border rounded-md px-4 py-3 text-sm flex items-start justify-between gap-3"
              :class="uiAlert.type === 'success'
                ? 'border-green-300 bg-green-50 text-green-800'
                : uiAlert.type === 'warning'
                  ? 'border-yellow-300 bg-yellow-50 text-yellow-800'
                  : 'border-red-300 bg-red-50 text-red-800'"
            >
              <div class="pr-2">
                {{ uiAlert.text }}
              </div>

              <button
                class="shrink-0 border px-2 py-1 rounded bg-white hover:bg-gray-50"
                @click="uiAlert = null"
                aria-label="Close alert"
              >
                ✕
              </button>
            </div>
          </div>
        </div>

        <div class="flex gap-3">
          <button
            @click="openAddModal"
            class="border border-[#1673ea] text-[#1673ea] font-semibold px-5 py-2.5 rounded-md shadow bg-white hover:bg-[#1673ea] hover:text-white transition"
          >
            Add Courier
          </button>

          <button
            @click="loadCouriers"
            class="border border-gray-300 text-gray-700 font-semibold px-5 py-2.5 rounded-md shadow bg-white hover:bg-gray-100 transition"
          >
            Refresh
          </button>
        </div>
      </div>

      <div class="flex justify-center !mt-5">
        <table class="hidden md:table table-fixed border-2 w-[90%] bg-white">
          <thead class="border-2 font-bold">
            <tr>
              <th class="border p-3 w-[12%]">ID</th>
              <th class="border w-[18%]">Created</th>
              <th class="border w-[35%]">Courier</th>
              <th class="border w-[15%]">Completed</th>
              <th class="border w-[10%]">Active</th>
              <th class="border w-[10%]">Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr v-if="!loading && paginatedItems.length === 0">
              <td class="border p-6 text-center text-gray-500" colspan="6">
                No couriers yet
              </td>
            </tr>

            <tr
              v-else
              v-for="row in paginatedItems"
              :key="row.id"
              class="text-center border"
            >
              <td class="border p-3">#{{ row.id }}</td>
              <td class="border">{{ row.created }}</td>
              <td class="border p-3 break-all whitespace-normal">{{ row.courier }}</td>
              <td class="border">{{ row.completed }}</td>
              <td class="border">{{ row.active }}</td>

              <td class="border p-3">
                <div class="flex items-center justify-center gap-6 cursor-pointer">
                  <img
                    src="@/assets/images/DeleteUser.png"
                    class="w-6"
                    title="Delete"
                    @click="deleteCourier(row.id)"
                  />
                  <img
                    src="@/assets/images/EditUser.png"
                    class="w-6"
                    title="Edit"
                    @click="openEditModal(row)"
                  />
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div class="md:hidden w-[90%]">
          <div v-if="!loading && paginatedItems.length === 0" class="border-2 bg-white p-6 text-center text-gray-500">
            No couriers yet
          </div>

          <div v-else class="flex flex-col gap-3">
            <div
              v-for="row in paginatedItems"
              :key="row.id"
              class="border-2 bg-white"
            >
              <div class="grid grid-cols-2 border-b">
                <div class="border-r p-3 text-xs font-bold text-gray-700">ID</div>
                <div class="p-3 text-sm text-right">#{{ row.id }}</div>
              </div>

              <div class="grid grid-cols-2 border-b">
                <div class="border-r p-3 text-xs font-bold text-gray-700">Created</div>
                <div class="p-3 text-sm text-right">{{ row.created }}</div>
              </div>

              <div class="grid grid-cols-2 border-b">
                <div class="border-r p-3 text-xs font-bold text-gray-700">Courier</div>
                <div class="p-3 text-sm text-right break-all whitespace-normal">{{ row.courier }}</div>
              </div>

              <div class="grid grid-cols-2 border-b">
                <div class="border-r p-3 text-xs font-bold text-gray-700">Completed</div>
                <div class="p-3 text-sm text-right">{{ row.completed }}</div>
              </div>

              <div class="grid grid-cols-2 border-b">
                <div class="border-r p-3 text-xs font-bold text-gray-700">Active</div>
                <div class="p-3 text-sm text-right">{{ row.active }}</div>
              </div>

              <div class="grid grid-cols-2">
                <div class="border-r p-3 text-xs font-bold text-gray-700">Actions</div>
                <div class="p-3 flex justify-end">
                  <div class="flex items-center gap-6 cursor-pointer">
                    <img
                      src="@/assets/images/DeleteUser.png"
                      class="w-6"
                      title="Delete"
                      @click="deleteCourier(row.id)"
                    />
                    <img
                      src="@/assets/images/EditUser.png"
                      class="w-6"
                      title="Edit"
                      @click="openEditModal(row)"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
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
      <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
        <h2 class="text-xl font-bold mb-4 text-center !mb-2">Add Courier</h2>

        <input
          v-model="form.courier"
          type="text"
          placeholder="Courier full name"
          class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;"
        />

        <div class="flex justify-between !mt-2">
          <button
            @click="showAddModal = false"
            class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Cancel
          </button>

          <button
            @click="addCourier"
            class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]"
          >
            Add
          </button>
        </div>
      </div>
    </div>

    <div v-if="showEditModal" class="fixed inset-0 bg-black/30 flex justify-center items-center z-50">
      <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
        <h2 class="text-xl font-bold mb-4 text-center">Edit Courier</h2>

        <input
          v-model="form.courier"
          type="text"
          placeholder="Courier full name"
          class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;"
        />

        <div class="flex justify-between">
          <button
            @click="showEditModal = false"
            class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Cancel
          </button>

          <button
            @click="saveEditedCourier"
            class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]"
          >
            Save
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
