<script setup lang="ts">
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import { ref, computed } from 'vue'

const items = ref([
  { id: "#12346", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12347", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12348", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12349", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12350", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12351", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12352", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12353", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12354", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12355", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12356", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12357", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12358", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
  { id: "#12359", created: "29.12.2025", customer: "Max Verstpen", address: "Paula leiņa 6, 12", time: "12:30 - 14:00" },
])

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

function deleteOrder(id: string) {
  items.value = items.value.filter(o => o.id !== id)
}

const showAddModal = ref(false)
const showEditModal = ref(false)
const editIndex = ref<number | null>(null)

const newOrder = ref({
  customer: "",
  address: "",
  time: "",
})

function generateId() {
  return "#" + Math.floor(10000 + Math.random() * 90000)
}

function openAddModal() {
  newOrder.value = { customer: "", address: "", time: "" }
  showAddModal.value = true
}

function addOrder() {
  if (!newOrder.value.customer || !newOrder.value.address) return

  items.value.push({
    id: generateId(),
    created: new Date().toLocaleDateString(),
    customer: newOrder.value.customer,
    address: newOrder.value.address,
    time: newOrder.value.time || "12:00 - 14:00"
  })

  showAddModal.value = false
  newOrder.value = { customer: "", address: "", time: "" }
}

function openEditModal(order: any, index: number) {
  editIndex.value = index
  newOrder.value = {
    customer: order.customer,
    address: order.address,
    time: order.time
  }
  showEditModal.value = true
}

function saveEditedOrder() {
  if (editIndex.value !== null) {
    items.value[editIndex.value] = {
      ...items.value[editIndex.value],
      customer: newOrder.value.customer,
      address: newOrder.value.address,
      time: newOrder.value.time
    }
  }

  showEditModal.value = false
  editIndex.value = null

  newOrder.value = { customer: "", address: "", time: "" }
}
</script>



<template>
  <div class="flex min-h-screen bg-white text-gray-900">
    <Sidebar />

    <div class="flex-1 px-12 py-12">
      <PageHeader title="Orders" />

      <!-- Button add-->
      <div class="mt-6 flex justify-end pr-[80px] pb-[10px]">
        <button @click="openAddModal" class="border border-[#1673ea] text-[#1673ea] font-semibold px-7 py-3.5 
                 rounded-md shadow bg-white hover:bg-[#1673ea] hover:text-white transition">
          Add Order
        </button>
      </div>

      <!-- Table -->
      <div class="flex justify-center">
        <table class="table-fixed border-2 w-[90%] bg-white">
          <thead class="border-2 font-bold">
            <tr>
              <th class="border p-3">ID</th>
              <th class="border">Created</th>
              <th class="border">Customer</th>
              <th class="border">Address</th>
              <th class="border">TimeLog</th>
              <th class="border">Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="(order, index) in paginatedItems" :key="order.id" class="text-center border">
              <td class="border p-3">{{ order.id }}</td>
              <td class="border">{{ order.created }}</td>
              <td class="border">{{ order.customer }}</td>
              <td class="border">{{ order.address }}</td>
              <td class="border">{{ order.time }}</td>
              <td class="border p-3">
                <div class="flex items-center justify-center gap-6 cursor-pointer">
                  <img src="@/assets/images/DeleteUser.png" class="w-6" @click="deleteOrder(order.id)" />
                  <img src="@/assets/images/EditUser.png" class="w-6" @click="openEditModal(order, index)" />
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Bot. but. -->
      <div class="flex justify-end pr-[80px] pt-[14px] items-center">
        <span style="margin-right: 14px;">
          {{ (currentPage - 1) * perPage + 1 }} -
          {{ Math.min(currentPage * perPage, items.length) }} of
          {{ items.length }}
        </span>

        <button @click="prevPage" :disabled="currentPage === 1"
          class="px-3 py-1 border rounded disabled:opacity-40">‹</button>

        <span class="mx-2 px-3 py-1 border rounded bg-gray-100">
          {{ currentPage }}
        </span>

        <button @click="nextPage" :disabled="currentPage === totalPages"
          class="px-3 py-1 border rounded disabled:opacity-40">›</button>
      </div>
    </div>

    <!--  Modal for adding -->
    <div v-if="showAddModal" class="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
      <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
        <h2 class="text-xl font-bold mb-4 text-center">Add New Order</h2>

        <input v-model="newOrder.customer" type="text" placeholder="Customer" class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;" />
        <input v-model="newOrder.address" type="text" placeholder="Address" class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;" />
        <input v-model="newOrder.time" type="text" placeholder="TimeLog (12:30 - 14:00)"
          class="w-full border p-2 rounded mb-4" style="margin-bottom: 10px;" />

        <div class="flex justify-between">
          <button @click="showAddModal = false" class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">Cancel</button>

          <button @click="addOrder" class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]">
            Add
          </button>
        </div>
      </div>
    </div>

    <!--  Modal for editing -->
    <div v-if="showEditModal" class="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
      <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
        <h2 class="text-xl font-bold mb-4 text-center">Edit Order</h2>

        <input v-model="newOrder.customer" type="text" placeholder="Customer" class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;" />
        <input v-model="newOrder.address" type="text" placeholder="Address" class="w-full border p-2 rounded mb-3"
          style="margin-bottom: 10px;" />
        <input v-model="newOrder.time" type="text" placeholder="TimeLog (12:30 - 14:00)"
          class="w-full border p-2 rounded mb-4" style="margin-bottom: 10px;" />

        <div class="flex justify-between">
          <button @click="showEditModal = false" class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">Cancel</button>

          <button @click="saveEditedOrder" class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]">
            Save
          </button>
        </div>
      </div>
    </div>

  </div>
</template>
