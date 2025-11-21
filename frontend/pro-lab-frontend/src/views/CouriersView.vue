<script setup lang="ts">
import Sidebar from '@/components/SideBar.vue'
import PageHeader from '@/components/PageHeader.vue'
import { ref, computed } from 'vue'

const items = ref([
    { id: "#321321", created: "29.12.2025", courier: "Max Verstpen", shipped: 5 },
    { id: "#321322", created: "29.12.2025", courier: "Max Verstpen", shipped: 1 },
    { id: "#321323", created: "29.12.2025", courier: "Max Verstpen", shipped: 2 },
    { id: "#321324", created: "29.12.2025", courier: "Max Verstpen", shipped: 14 },
    { id: "#321325", created: "29.12.2025", courier: "Max Verstpen", shipped: 42 },
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

// Delete
function deleteOrder(id: string) {
    items.value = items.value.filter(o => o.id !== id)
}

// Modal
const showModal = ref(false)
const showEditModal = ref(false)
const editIndex = ref<number | null>(null)

const newOrder = ref({
    id: "",
    created: "",
    courier: "",
    shipped: 0,
})

// Id generation
function generateId() {
    return "#" + Math.floor(10000 + Math.random() * 90000)
}

// Add order
function addOrder() {
    if (!newOrder.value.courier) return

    items.value.push({
        id: generateId(),
        created: new Date().toLocaleDateString(),
        courier: newOrder.value.courier,
        shipped: Number(newOrder.value.shipped),
    })

    showModal.value = false

    newOrder.value = {
        id: "",
        created: "",
        courier: "",
        shipped: 0,
    }
}

// Open Edit
function openEditModal(order: any, index: number) {
    editIndex.value = index
    newOrder.value = {
        id: order.id,
        created: order.created,
        courier: order.courier,
        shipped: order.shipped,
    }
    showEditModal.value = true
}

// Save Edit
function saveEditedOrder() {
    if (editIndex.value !== null) {
        items.value[editIndex.value] = {
            ...items.value[editIndex.value],
            courier: newOrder.value.courier,
            shipped: Number(newOrder.value.shipped)
        }
    }

    showEditModal.value = false
    editIndex.value = null

    newOrder.value = {
        id: "",
        created: "",
        courier: "",
        shipped: 0,
    }
}
</script>



<template>
    <div class="flex min-h-screen bg-white text-gray-900">
        <Sidebar />

        <div class="flex-1 px-12 py-12">
            <PageHeader title="Couriers" />

            <!-- Button add-->
            <div class="mt-6 flex justify-end pr-[80px] pb-[10px]">
                <button @click="showModal = true" class="border border-[#1673ea] text-[#1673ea] font-semibold px-7 py-3.5 
                 rounded-md shadow bg-white hover:bg-[#1673ea] hover:text-white transition">
                    Add Courier
                </button>
            </div>

            <!-- Table -->
            <div class="flex justify-center">
                <table class="table-fixed border-2 w-[90%] bg-white">
                    <thead class="border-2 font-bold">
                        <tr>
                            <th class="border p-3">ID</th>
                            <th class="border">Created</th>
                            <th class="border">Courier</th>
                            <th class="border">Shipped (overall)</th>
                            <th class="border">Actions</th>
                        </tr>
                    </thead>

                    <tbody>
                        <tr v-for="(order, index) in paginatedItems" :key="order.id" class="text-center border">
                            <td class="border p-3">{{ order.id }}</td>
                            <td class="border">{{ order.created }}</td>
                            <td class="border">{{ order.courier }}</td>
                            <td class="border">{{ order.shipped }}</td>
                            <td class="border p-3">
                                <div class="flex items-center justify-center gap-6 cursor-pointer">
                                    <img src="@/assets/images/DeleteUser.png" class="w-6"
                                        @click="deleteOrder(order.id)" />
                                    <img src="@/assets/images/EditUser.png" class="w-6"
                                        @click="openEditModal(order, index)" />
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

        <!--  Modal Add -->
        <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
            <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
                <h2 class="text-xl font-bold mb-4 text-center">Add Courier</h2>

                <input v-model="newOrder.courier" type="text" placeholder="Courier"
                    class="w-full border p-2 rounded mb-3" style="margin-bottom: 10px;" />
                <input v-model="newOrder.shipped" type="number" placeholder="Shipped Count"
                    class="w-full border p-2 rounded mb-3" style="margin-bottom: 10px;" />

                <div class="flex justify-between">
                    <button @click="showModal = false"
                        class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">Cancel</button>

                    <button @click="addOrder" class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]">
                        Add
                    </button>
                </div>
            </div>
        </div>

        <!--  Modal Edit -->
        <div v-if="showEditModal" class="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
            <div class="bg-white p-6 rounded-lg shadow-lg w-[400px]">
                <h2 class="text-xl font-bold mb-4 text-center">Edit Courier</h2>

                <input v-model="newOrder.courier" type="text" placeholder="Courier"
                    class="w-full border p-2 rounded mb-3" style="margin-bottom: 10px;" />
                <input v-model="newOrder.shipped" type="number" placeholder="Shipped Count"
                    class="w-full border p-2 rounded mb-3" style="margin-bottom: 10px;" />

                <div class="flex justify-between">
                    <button @click="showEditModal = false"
                        class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">Cancel</button>

                    <button @click="saveEditedOrder"
                        class="px-4 py-2 bg-[#1673ea] text-white rounded hover:bg-[#105fc6]">
                        Save
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>
