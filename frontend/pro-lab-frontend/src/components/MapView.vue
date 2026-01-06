<template>
  <div ref="mapContainer" class="w-full h-full rounded-lg relative"></div>
</template>

<script setup lang="ts">
import "mapbox-gl/dist/mapbox-gl.css";
import { onMounted, onBeforeUnmount, ref, watch } from "vue";
import mapboxgl from "mapbox-gl";
import type { CoordinateDto } from "@/api-client/clients";

type Props = {
  coordinates: CoordinateDto[];
  routeGeometry?: CoordinateDto[] | null;
  visitOrder?: number[] | null;
  disabled?: boolean;
};

const props = defineProps<Props>();
const emit = defineEmits<{
  (e: "update:coordinates", value: CoordinateDto[]): void;
}>();

const mapContainer = ref<HTMLElement | null>(null);
let map: mapboxgl.Map | null = null;

let markers: mapboxgl.Marker[] = [];

let orderMarkers: mapboxgl.Marker[] = [];

mapboxgl.accessToken =
  "pk.eyJ1Ijoibmlja2FpIiwiYSI6ImNtaGFnOXcwMjAzYmIycnNjM2tvMzd4eWwifQ.FH6XJMiQG-4pmHwvtpHcWQ";

function setCoordinates(next: CoordinateDto[]) {
  emit("update:coordinates", next);
}

function addPoint(lng: number, lat: number) {
  if (props.disabled) return;
  setCoordinates([...props.coordinates, { longitude: lng, latitude: lat }]);
}

function removePoint(index: number) {
  setCoordinates(props.coordinates.filter((_, i) => i !== index));
}

function updatePoint(index: number, lng: number, lat: number) {
  setCoordinates(
    props.coordinates.map((c, i) =>
      i === index ? { longitude: lng, latitude: lat } : c
    )
  );
}

function rebuildMarkers() {
  for (const m of markers) m.remove();
  markers = [];
  if (!map) return;

  props.coordinates.forEach((c, idx) => {
    const el = document.createElement("div");
    el.style.width = "18px";
    el.style.height = "18px";
    el.style.borderRadius = "9999px";
    el.style.border = "2px solid white";
    el.style.boxShadow = "0 1px 6px rgba(0,0,0,0.25)";
    el.style.cursor = "pointer";
    el.style.background = idx === 0 ? "#1673ea" : "#f97316";

    const marker = new mapboxgl.Marker({ element: el, draggable: true })
      .setLngLat([c.longitude, c.latitude])
      .addTo(map);

    el.addEventListener("click", (e) => {
      e.stopPropagation();
      removePoint(idx);
    });

    marker.on("dragend", () => {
      const p = marker.getLngLat();
      updatePoint(idx, p.lng, p.lat);
    });

    markers.push(marker);
  });
}

const ROUTE_SOURCE_ID = "route-src";
const ROUTE_LINE_LAYER_ID = "route-line";
const ROUTE_POINTS_LAYER_ID = "route-points";

function ensureRouteLayers() {
  if (!map) return;
  if (!map.getSource(ROUTE_SOURCE_ID)) {
    map.addSource(ROUTE_SOURCE_ID, {
      type: "geojson",
      data: {
        type: "FeatureCollection",
        features: [],
      },
    });

    map.addLayer({
      id: ROUTE_LINE_LAYER_ID,
      type: "line",
      source: ROUTE_SOURCE_ID,
      filter: ["==", ["geometry-type"], "LineString"],
      layout: {
        "line-join": "round",
        "line-cap": "round",
      },
      paint: {
        "line-width": 5,
        "line-opacity": 0.85,
      },
    });

    map.addLayer({
      id: ROUTE_POINTS_LAYER_ID,
      type: "circle",
      source: ROUTE_SOURCE_ID,
      filter: ["==", ["geometry-type"], "Point"],
      paint: {
        "circle-radius": 3,
        "circle-opacity": 0.8,
      },
    });
  }
}

function setRouteGeometry(geometry: CoordinateDto[] | null | undefined) {
  if (!map) return;
  const src = map.getSource(ROUTE_SOURCE_ID) as mapboxgl.GeoJSONSource | undefined;
  if (!src) return;

  if (!geometry || geometry.length < 2) {
    src.setData({ type: "FeatureCollection", features: [] });
    return;
  }

  const line = {
    type: "Feature" as const,
    properties: {},
    geometry: {
      type: "LineString" as const,
      coordinates: geometry.map((p) => [p.longitude, p.latitude]),
    },
  };

  const points = geometry.map((p) => ({
    type: "Feature" as const,
    properties: {},
    geometry: {
      type: "Point" as const,
      coordinates: [p.longitude, p.latitude],
    },
  }));

  src.setData({
    type: "FeatureCollection",
    features: [line, ...points],
  });

  const bounds = geometry.reduce((b, p) => {
    return b.extend([p.longitude, p.latitude]);
  }, new mapboxgl.LngLatBounds([geometry[0].longitude, geometry[0].latitude], [geometry[0].longitude, geometry[0].latitude]));

  map.fitBounds(bounds, { padding: 60, duration: 500 });
}

function clearOrderMarkers() {
  for (const m of orderMarkers) m.remove();
  orderMarkers = [];
}

function renderVisitOrderMarkers() {
  if (!map) return;
  clearOrderMarkers();

  if (!props.visitOrder || props.visitOrder.length === 0) return;
  if (!props.coordinates || props.coordinates.length === 0) return;

  props.visitOrder.forEach((pointIndex, order) => {
    const p = props.coordinates[pointIndex];
    if (!p) return;

    const el = document.createElement("div");
    el.style.width = "26px";
    el.style.height = "26px";
    el.style.borderRadius = "9999px";
    el.style.border = "2px solid white";
    el.style.boxShadow = "0 2px 10px rgba(0,0,0,0.25)";
    el.style.display = "flex";
    el.style.alignItems = "center";
    el.style.justifyContent = "center";
    el.style.fontSize = "12px";
    el.style.fontWeight = "700";
    el.style.color = "white";
    el.style.background = order === 0 ? "#1673ea" : "#111827";

    el.textContent = String(order);

    const m = new mapboxgl.Marker({ element: el, draggable: false })
      .setLngLat([p.longitude, p.latitude])
      .addTo(map);

    orderMarkers.push(m);
  });
}

onMounted(() => {
  map = new mapboxgl.Map({
    container: mapContainer.value!,
    style: "mapbox://styles/mapbox/light-v11",
    center: [24.1051864, 56.9496487],
    zoom: 10,
  });

  map.addControl(new mapboxgl.NavigationControl());

  map.on("click", (e) => {
    addPoint(e.lngLat.lng, e.lngLat.lat);
  });

  map.on("load", () => {
    rebuildMarkers();

    ensureRouteLayers();
    setRouteGeometry(props.routeGeometry);

    renderVisitOrderMarkers();
  });
});

watch(
  () => props.coordinates,
  () => {
    rebuildMarkers();
    renderVisitOrderMarkers();
  },
  { deep: true }
);

watch(
  () => props.routeGeometry,
  (g) => {
    if (!map) return;
    ensureRouteLayers();
    setRouteGeometry(g);
  },
  { deep: true }
);

watch(
  () => props.visitOrder,
  () => {
    renderVisitOrderMarkers();
  },
  { deep: true }
);

onBeforeUnmount(() => {
  clearOrderMarkers();
  for (const m of markers) m.remove();
  markers = [];
  if (map) map.remove();
  map = null;
});
</script>

<style>
.mapboxgl-canvas {
  border-radius: 0.5rem;
}
</style>
