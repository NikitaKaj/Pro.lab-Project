import { createRouter, createWebHistory } from 'vue-router'

import RegistrationView from '@/views/RegistrationView.vue' 
import DashboardView from '@/views/DashboardView.vue'
import RoutesView from '@/views/RoutesView.vue'

const routes = [
  { path: '/', name: 'login', component: RegistrationView },
  { path: '/dashboard', name: 'dashboard', component: DashboardView },
  { path: '/routes', name: 'routes', component: RoutesView},
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
