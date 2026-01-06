<script setup lang="ts">
import { computed, ref, onMounted, watch } from "vue";
import { Bars3Icon } from "@heroicons/vue/24/outline";

import Sidebar from "@/components/SideBar.vue";
import MapView from "@/components/MapView.vue";
import PageHeader from "@/components/PageHeader.vue";

import {
  RoutesClient,
  type CoordinateDto,
  SelectionStrategy,
  OptimizationAlgorithm,
  type OptimizeRouteRequest,
  CouriersClient,
  type CourierResponse,
} from "@/api-client/clients";

const sidebarOpen = ref(false);

const selectedCoordinates = ref<CoordinateDto[]>([]);
const startIndex = ref(0);

const selectionStrategy = ref<SelectionStrategy>(SelectionStrategy.Balanced);
const algorithm = ref<OptimizationAlgorithm>(OptimizationAlgorithm.WithAlternatives);

const optimizeResult = ref<any>(null);
const loading = ref(false);
const errorText = ref<string | null>(null);

const baseUrl = import.meta.env.VITE_API_BASE_URL ?? "https://localhost:5001";
const routesClient = new RoutesClient(baseUrl);
const couriersClient = new CouriersClient(baseUrl);

const couriers = ref<CourierResponse[]>([]);
const couriersLoading = ref(false);
const couriersError = ref<string | null>(null);

const selectedCourierId = ref<number | null>(null);

const eligibleCouriers = computed(() =>
  (couriers.value ?? []).filter((c) => (c.activeOrdersCount ?? 0) > 1)
);

function courierLabel(c: CourierResponse) {
  return `${c.fullName} (#${c.courierId}) — active: ${c.activeOrdersCount}`;
}

async function loadCouriers() {
  couriersLoading.value = true;
  couriersError.value = null;
  try {
    const data = await couriersClient.get();
    couriers.value = data ?? [];
  } catch (e: any) {
    couriersError.value = e?.message ?? "Failed to load couriers";
  } finally {
    couriersLoading.value = false;
  }
}

const canOptimize = computed(() => {
  if (selectedCourierId.value != null) return true;
  return selectedCoordinates.value.length >= 2;
});

watch(selectedCourierId, (id) => {
  errorText.value = null;
  optimizeResult.value = null;

  if (id != null) {
    selectedCoordinates.value = [];
    startIndex.value = 0;
  }
});

async function generateRouteManual() {
  if (selectedCoordinates.value.length < 2) return;

  loading.value = true;
  errorText.value = null;
  optimizeResult.value = null;

  const request: OptimizeRouteRequest = {
    coordinates: selectedCoordinates.value,
    startIndex: startIndex.value,
    algorithm: algorithm.value,
    selectionStrategy: selectionStrategy.value,
  };

  try {
    const res = await routesClient.login(request);
    optimizeResult.value = res;
  } catch (e: any) {
    errorText.value = e?.detail ?? e?.title ?? e?.message ?? JSON.stringify(e);
  } finally {
    loading.value = false;
  }
}

async function generateRouteByCourier() {
  if (selectedCourierId.value == null) return;

  loading.value = true;
  errorText.value = null;
  optimizeResult.value = null;

  try {
    const res = await routesClient.getRoute({
      courierId: selectedCourierId.value,
      algorithm: algorithm.value,
      selectionStrategy: selectionStrategy.value,
    });
    optimizeResult.value = res;

    const coords = (res as any)?.coordinates;
    if (Array.isArray(coords) && coords.length) {
      selectedCoordinates.value = coords.map((c: any) => ({
        longitude: c.longitude,
        latitude: c.latitude,
      }));
      startIndex.value = 0;
    } else {
      selectedCoordinates.value = [];
      startIndex.value = 0;
    }
  } catch (e: any) {
    errorText.value = e?.detail ?? e?.title ?? e?.message ?? JSON.stringify(e);
  } finally {
    loading.value = false;
  }
}

async function generateOptimalRoute() {
  if (selectedCourierId.value != null) return generateRouteByCourier();
  return generateRouteManual();
}

function clearPoints() {
  selectedCoordinates.value = [];
  startIndex.value = 0;
  optimizeResult.value = null;
  errorText.value = null;
}

onMounted(async () => {
  await loadCouriers();
});
</script>

<template>
  <div class="min-h-screen bg-white text-gray-900 md:flex">
    <Sidebar v-model:open="sidebarOpen" />

    <div class="flex-1 px-4 py-6 sm:px-6 sm:py-8 lg:px-12 lg:py-12">
      <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between mb-6">
        <div class="flex items-center gap-3">
          <button
            class="md:hidden p-2 rounded-md border border-gray-200 bg-white"
            @click="sidebarOpen = true"
            aria-label="Open menu"
          >
            <Bars3Icon class="w-6 h-6" />
          </button>

          <div class="flex-1">
            <PageHeader title="Routes" />
          </div>
        </div>

        <div class="flex flex-col sm:flex-row gap-3 w-full sm:w-auto">
          <button
            class="w-full sm:w-auto bg-gray-100 hover:bg-gray-200 text-gray-900 font-semibold px-5 py-3 rounded-md border border-gray-200"
            @click="clearPoints"
          >
            Clear
          </button>

          <button
            class="w-full sm:w-auto bg-[#1673ea] hover:bg-[#105fc6] disabled:bg-[#9bbff3] text-white font-semibold px-6 py-3 rounded-md shadow"
            :disabled="!canOptimize || loading"
            @click="generateOptimalRoute"
          >
            {{ loading ? "Generating..." : "Generate Optimal Route" }}
          </button>
        </div>
      </div>

      <div class="flex flex-col lg:flex-row gap-6">
        <div
          class="w-full lg:w-2/3 bg-white rounded-lg border border-gray-200 shadow-sm h-[420px] sm:h-[600px] flex items-center justify-center overflow-hidden"
        >
          <MapView
            v-model:coordinates="selectedCoordinates"
            :route-geometry="optimizeResult?.fullGeometry ?? null"
            :visit-order="optimizeResult?.visitOrder ?? null"
            :disabled="selectedCourierId !== null"
          />
        </div>

        <div class="w-full lg:w-1/3 bg-white rounded-lg border border-gray-200 shadow-sm p-4">
          <h2 class="text-lg text-black font-semibold mb-2">Route mode</h2>

          <div class="text-sm text-gray-600 mb-4">
            <div class="mb-2">
              <span class="font-medium">Manual points:</span>
              pick points on map (need 2+), then generate.
            </div>
            <div>
              <span class="font-medium">Courier mode:</span>
              pick courier (with &gt; 1 active order)
            </div>
          </div>

          <div class="mb-4">
            <label class="text-sm text-gray-700 font-medium">Courier (optional)</label>

            <select
              class="w-full border border-gray-200 rounded-md px-3 py-2 bg-white disabled:opacity-60"
              v-model="selectedCourierId"
              :disabled="couriersLoading"
            >
              <option :value="null">Manual points (no courier)</option>

              <option v-if="eligibleCouriers.length === 0" :value="null" disabled>
                No couriers with &gt; 1 active order
              </option>

              <option
                v-for="c in eligibleCouriers"
                :key="c.courierId"
                :value="c.courierId"
              >
                {{ courierLabel(c) }}
              </option>
            </select>

            <div class="mt-1 text-xs text-gray-600">
              <span v-if="couriersLoading">Loading couriers…</span>
              <span v-else-if="couriersError" class="text-red-600">{{ couriersError }}</span>
              <span v-else>&nbsp;</span>
            </div>
          </div>

          <h2 class="text-lg text-black font-semibold mb-2">Selected points</h2>
          <p class="text-sm text-gray-600 mb-4">
            <span v-if="selectedCourierId != null">
              Courier selected, manual point selection disabled.
            </span>
            <span v-else>
              Click on map to add. Click marker to remove. Drag to adjust.
            </span>
          </p>

          <div class="mb-4">
            <label class="text-sm text-gray-700 font-medium">Selection strategy</label>
            <select
              class="mt-1 w-full border border-gray-200 rounded-md px-3 py-2"
              v-model.number="selectionStrategy"
            >
              <option :value="SelectionStrategy.Fastest">Fastest</option>
              <option :value="SelectionStrategy.Shortest">Shortest</option>
              <option :value="SelectionStrategy.Balanced">Balanced</option>
              <option :value="SelectionStrategy.LightestWeight">LightestWeight</option>
              <option :value="SelectionStrategy.TimeOfDay">TimeOfDay</option>
            </select>
          </div>

          <div class="mb-6">
            <label class="text-sm text-gray-700 font-medium">Algorithm</label>
            <select
              class="mt-1 w-full border border-gray-200 rounded-md px-3 py-2"
              v-model.number="algorithm"
            >
              <option :value="OptimizationAlgorithm.NearestNeighbor">NearestNeighbor</option>
              <option :value="OptimizationAlgorithm.WithAlternatives">WithAlternatives</option>
            </select>
          </div>

          <div v-if="errorText" class="mt-4 text-sm text-red-600">
            {{ errorText }}
          </div>

          <div class="mt-4 text-xs text-gray-600">
            <div>
              Manual points selected:
              <span class="font-semibold">{{ selectedCoordinates.length }}</span>
            </div>
            <div>
              Mode:
              <span class="font-semibold">
                {{ selectedCourierId != null ? "Courier mode" : "Manual mode" }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
