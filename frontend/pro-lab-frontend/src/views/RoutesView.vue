<script setup lang="ts">
import { computed, ref } from "vue";
import Sidebar from "@/components/SideBar.vue";
import MapView from "@/components/MapView.vue";
import PageHeader from "@/components/PageHeader.vue";

import {
  RoutesClient,
  type CoordinateDto,
  SelectionStrategy,
  OptimizationAlgorithm,
  type OptimizeRouteRequest,
} from "@/api-client/clients";

const selectedCoordinates = ref<CoordinateDto[]>([]);

const startIndex = ref(0);

const selectionStrategy = ref<SelectionStrategy>(SelectionStrategy.Balanced);

const algorithm = ref<OptimizationAlgorithm>(OptimizationAlgorithm.WithAlternatives);

const optimizeResult = ref<any>(null);
const loading = ref(false);
const errorText = ref<string | null>(null);

const canOptimize = computed(() => selectedCoordinates.value.length >= 2);

const routesClient = new RoutesClient(import.meta.env.VITE_API_BASE_URL ?? "https://localhost:5001");

async function generateOptimalRoute() {
  if (!canOptimize.value) return;

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
    errorText.value =
      e?.detail ??
      e?.title ??
      e?.message ??
      JSON.stringify(e);
  } finally {
    loading.value = false;
  }
}

function clearPoints() {
  selectedCoordinates.value = [];
  startIndex.value = 0;
  optimizeResult.value = null;
  errorText.value = null;
}
</script>

<template>
  <div class="flex min-h-screen bg-white text-gray-900">
    <Sidebar />

    <div class="flex-1 px-12 py-12">
      <div class="flex items-center justify-between mb-8">
        <PageHeader title="Routes" />

        <div class="flex gap-3">
          <button
            class="bg-gray-100 hover:bg-gray-200 text-gray-900 font-semibold px-5 py-3.5 rounded-md border border-gray-200"
            @click="clearPoints"
          >
            Clear
          </button>

          <button
            class="bg-[#1673ea] hover:bg-[#105fc6] disabled:bg-[#9bbff3] text-white font-semibold px-7 py-3.5 rounded-md shadow"
            :disabled="!canOptimize || loading"
            @click="generateOptimalRoute"
          >
            {{ loading ? "Generating..." : "Generate Optimal Route" }}
          </button>
        </div>
      </div>

      <div class="flex gap-6">
        <div class="w-2/3 bg-white rounded-lg border border-gray-200 shadow-sm h-[600px] flex items-center justify-center">
          <MapView
            v-model:coordinates="selectedCoordinates"
            :route-geometry="optimizeResult?.fullGeometry ?? null"
            :visit-order="optimizeResult?.visitOrder ?? null"
          />
        </div>

        <div class="w-1/3 bg-white rounded-lg border border-gray-200 shadow-sm p-4">
          <h2 class="text-lg text-black font-semibold mb-2">Selected points</h2>
          <p class="text-sm text-gray-600 mb-4">
            Click on map to add. Click marker to remove. Drag to adjust.
          </p>

          <div class="mb-4">
            <label class="text-sm text-gray-700 font-medium">Start index</label>
            <select
              class="mt-1 w-full border border-gray-200 rounded-md px-3 py-2"
              v-model.number="startIndex"
              :disabled="selectedCoordinates.length === 0"
            >
              <option v-for="(_, idx) in selectedCoordinates" :key="idx" :value="idx">
                {{ idx }} ({{ selectedCoordinates[idx].longitude.toFixed(5) }}, {{ selectedCoordinates[idx].latitude.toFixed(5) }})
              </option>
            </select>
          </div>

          <div class="mb-4">
            <label class="text-sm text-gray-700 font-medium">Selection strategy</label>
            <select class="mt-1 w-full border border-gray-200 rounded-md px-3 py-2" v-model.number="selectionStrategy">
              <option :value="SelectionStrategy.Fastest">Fastest</option>
              <option :value="SelectionStrategy.Shortest">Shortest</option>
              <option :value="SelectionStrategy.Balanced">Balanced</option>
              <option :value="SelectionStrategy.LightestWeight">LightestWeight</option>
              <option :value="SelectionStrategy.TimeOfDay">TimeOfDay</option>
            </select>
          </div>

          <div class="mb-6">
            <label class="text-sm text-gray-700 font-medium">Algorithm</label>
            <select class="mt-1 w-full border border-gray-200 rounded-md px-3 py-2" v-model.number="algorithm">
              <option :value="OptimizationAlgorithm.NearestNeighbor">NearestNeighbor</option>
              <option :value="OptimizationAlgorithm.WithAlternatives">WithAlternatives</option>
            </select>
          </div>

          <div v-if="errorText" class="mt-4 text-sm text-red-600">
            {{ errorText }}
          </div>

          <!-- <div v-if="optimizeResult" class="mt-4">
            <h3 class="text-md text-black font-semibold mb-2">Result</h3>
            <pre class="text-xs bg-gray-50 border border-gray-200 rounded-md p-3 overflow-auto max-h-[220px]">
              {{ JSON.stringify(optimizeResult, null, 2) }}
            </pre>
          </div> -->
        </div>
      </div>
    </div>
  </div>
</template>
