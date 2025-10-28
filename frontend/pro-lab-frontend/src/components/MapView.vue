<template>
  <div ref="mapContainer" class="w-full h-full rounded-lg"></div>
</template>

<script setup>
import { onMounted, ref, onBeforeUnmount } from "vue";
import mapboxgl from "mapbox-gl";

const mapContainer = ref(null);
let map = null;

mapboxgl.accessToken = "pk.eyJ1Ijoibmlja2FpIiwiYSI6ImNtaGFnOXcwMjAzYmIycnNjM2tvMzd4eWwifQ.FH6XJMiQG-4pmHwvtpHcWQ";

onMounted(() => {
  map = new mapboxgl.Map({
    container: mapContainer.value,
    style: "mapbox://styles/mapbox/light-v11",
    center: [24.1051864, 56.9496487],
    zoom: 10,
  });

  map.addControl(new mapboxgl.NavigationControl());
});

onBeforeUnmount(() => {
  if (map) map.remove();
});
</script>

<style>
.mapboxgl-canvas {
  border-radius: 0.5rem;
}
</style>
