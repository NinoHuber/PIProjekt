<template>
  <div class="planner-container">
    <header class="planner-header">
      <h1>Wedding Planner Dashboard</h1>
      <p>Configure the big day, search providers, and calculate agency commissions.</p>
    </header>

    <div class="planner-layout">
      <div class="form-column">
        <div class="card">
          <label class="label-heading">Wedding Date</label>
          <input type="date" v-model="weddingDate" class="input-field" />
        </div>

        <div class="card space-y">
          <h2>Vendor Selection</h2>

          <div class="custom-dropdown-wrapper">
            <label class="label-heading">Restaurant</label>
            <div class="dropdown-trigger" @click="toggleDropdown('restaurant', 'Restaurant')">
              <span>
                {{ selectedRestaurant ? `${selectedRestaurant.name} ($${selectedRestaurant.price})` : 'Select a restaurant...' }}
              </span>
              <span class="arrow-icon" :class="{ open: dropdownState.restaurant }">▼</span>
            </div>
            
            <div v-if="dropdownState.restaurant" class="dropdown-panel">
              <input type="text" v-model="filters.restaurant" placeholder="Search restaurants..." class="search-input" @input="fetchVendors('restaurant', 'Restaurant')" />
              <div class="dropdown-options">
                <div 
                  v-for="r in vendorsData.restaurant.items" :key="r.id" 
                  class="option-item" :class="{ selected: selectedRestaurant?.id === r.id }"
                  @click="selectItem('restaurant', r)"
                >
                  <strong>{{ r.name }}</strong> (${{ r.price }}) — {{ r.hasHall ? 'Has Wedding Hall' : 'No Hall' }}
                </div>
                <div v-if="vendorsData.restaurant.items.length === 0" class="no-results">No restaurants found.</div>
              </div>
              <div class="pagination-bar" v-if="vendorsData.restaurant.totalPages > 1">
                <button :disabled="pages.restaurant === 1" @click="changePage('restaurant', 'Restaurant', -1)">Prev</button>
                <span>Page {{ pages.restaurant }} of {{ vendorsData.restaurant.totalPages }}</span>
                <button :disabled="pages.restaurant === vendorsData.restaurant.totalPages" @click="changePage('restaurant', 'Restaurant', 1)">Next</button>
              </div>
            </div>
          </div>

          <div class="custom-dropdown-wrapper">
            <label class="label-heading">Pastry (Cookies & Cake)</label>
            <div class="dropdown-trigger" @click="toggleDropdown('pastry', 'Pastry')">
              <span>
                {{ selectedPastry ? `${selectedPastry.name} ($${selectedPastry.price})` : 'Select a pastry shop...' }}
              </span>
              <span class="arrow-icon" :class="{ open: dropdownState.pastry }">▼</span>
            </div>
            
            <div v-if="dropdownState.pastry" class="dropdown-panel">
              <input type="text" v-model="filters.pastry" placeholder="Search pastries..." class="search-input" @input="fetchVendors('pastry', 'Pastry')" />
              <div class="dropdown-options">
                <div 
                  v-for="p in vendorsData.pastry.items" :key="p.id" 
                  class="option-item" :class="{ selected: selectedPastry?.id === p.id }"
                  @click="selectItem('pastry', p)"
                >
                  <strong>{{ p.name }}</strong> (${{ p.price }})
                </div>
                <div v-if="vendorsData.pastry.items.length === 0" class="no-results">No pastries found.</div>
              </div>
              <div class="pagination-bar" v-if="vendorsData.pastry.totalPages > 1">
                <button :disabled="pages.pastry === 1" @click="changePage('pastry', 'Pastry', -1)">Prev</button>
                <span>Page {{ pages.pastry }} of {{ vendorsData.pastry.totalPages }}</span>
                <button :disabled="pages.pastry === vendorsData.pastry.totalPages" @click="changePage('pastry', 'Pastry', 1)">Next</button>
              </div>
            </div>
          </div>

          <div class="custom-dropdown-wrapper">
            <label class="label-heading">Florist</label>
            <div class="dropdown-trigger" @click="toggleDropdown('florist', 'Florist')">
              <span>
                {{ selectedFlorist ? `${selectedFlorist.name} ($${selectedFlorist.price})` : 'Select a florist...' }}
              </span>
              <span class="arrow-icon" :class="{ open: dropdownState.florist }">▼</span>
            </div>
            
            <div v-if="dropdownState.florist" class="dropdown-panel">
              <input type="text" v-model="filters.florist" placeholder="Search florists..." class="search-input" @input="fetchVendors('florist', 'Florist')" />
              <div class="dropdown-options">
                <div 
                  v-for="f in vendorsData.florist.items" :key="f.id" 
                  class="option-item" :class="{ selected: selectedFlorist?.id === f.id }"
                  @click="selectItem('florist', f)"
                >
                  <strong>{{ f.name }}</strong> (${{ f.price }})
                </div>
                <div v-if="vendorsData.florist.items.length === 0" class="no-results">No florists found.</div>
              </div>
              <div class="pagination-bar" v-if="vendorsData.florist.totalPages > 1">
                <button :disabled="pages.florist === 1" @click="changePage('florist', 'Florist', -1)">Prev</button>
                <span>Page {{ pages.florist }} of {{ vendorsData.florist.totalPages }}</span>
                <button :disabled="pages.florist === vendorsData.florist.totalPages" @click="changePage('florist', 'Florist', 1)">Next</button>
              </div>
            </div>
          </div>

          <div class="custom-dropdown-wrapper">
            <label class="label-heading">Wedding Hall Owner</label>
            <div class="dropdown-trigger" @click="toggleDropdown('hall', 'Wedding Hall')">
              <span>
                {{ selectedHall ? `${selectedHall.name} ($${selectedHall.price})` : 'Select a hall...' }}
              </span>
              <span class="arrow-icon" :class="{ open: dropdownState.hall }">▼</span>
            </div>
            
            <div v-if="dropdownState.hall" class="dropdown-panel">
              <input type="text" v-model="filters.hall" placeholder="Search wedding halls..." class="search-input" @input="fetchVendors('hall', 'Wedding Hall')" />
              <div class="dropdown-options">
                <div 
                  v-for="h in vendorsData.hall.items" :key="h.id" 
                  class="option-item" :class="{ selected: selectedHall?.id === h.id }"
                  @click="selectItem('hall', h)"
                >
                  <strong>{{ h.name }}</strong> (${{ h.price }})
                </div>
                <div v-if="vendorsData.hall.items.length === 0" class="no-results">No halls found.</div>
              </div>
              <div class="pagination-bar" v-if="vendorsData.hall.totalPages > 1">
                <button :disabled="pages.hall === 1" @click="changePage('hall', 'Wedding Hall', -1)">Prev</button>
                <span>Page {{ pages.hall }} of {{ vendorsData.hall.totalPages }}</span>
                <button :disabled="pages.hall === vendorsData.hall.totalPages" @click="changePage('hall', 'Wedding Hall', 1)">Next</button>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label class="label-heading">Music Bands (Select one or more)</label>
            <p class="help-text">Bands with conflicting schedules will automatically deselect previous choices.</p>
            
            <input type="text" v-model="filters.band" placeholder="Search bands..." class="search-input band-search" @input="fetchVendors('band', 'Band')" />
            
            <div class="checkbox-list">
              <div v-for="b in vendorsData.band.items" :key="b.id" class="checkbox-item">
                <input 
                  type="checkbox" 
                  :id="'band-' + b.id"
                  :checked="selectedBands.some(sb => sb.id === b.id)"
                  @change="toggleBand(b)"
                  class="checkbox-input"
                />
                <label :for="'band-' + b.id" class="checkbox-label">
                  <strong>{{ b.name }}</strong> (${{ b.price }}) 
                  <span class="schedule-text">Schedule: {{ b.startTime }} - {{ b.endTime }}</span>
                </label>
              </div>
              <div v-if="vendorsData.band.items.length === 0" class="no-results">No bands found.</div>
            </div>
            
            <div class="pagination-bar" v-if="vendorsData.band.totalPages > 1">
              <button :disabled="pages.band === 1" @click="changePage('band', 'Band', -1)">Prev</button>
              <span>Page {{ pages.band }} of {{ vendorsData.band.totalPages }}</span>
              <button :disabled="pages.band === vendorsData.band.totalPages" @click="changePage('band', 'Band', 1)">Next</button>
            </div>
          </div>
        </div>
      </div>

      <div class="summary-column">
        <div class="card sticky-card">
          <h2>Cost Summary</h2>
          <div class="summary-row date-row">
            <span>Date:</span>
            <strong>{{ weddingDate || 'Not set' }}</strong>
          </div>
          <div class="financial-details">
            <div class="summary-row">
              <span>Vendor Subtotal:</span>
              <span class="price-value">${{ financialSummary.subtotal.toFixed(2) }}</span>
            </div>
            <div class="summary-row commission-text">
              <span>Agency Commission:</span>
              <span>+${{ financialSummary.totalCommission.toFixed(2) }}</span>
            </div>
            <div class="summary-row total-row">
              <span>Total Price:</span>
              <span>${{ financialSummary.total.toFixed(2) }}</span>
            </div>
          </div>
          <button @click="submitBooking" :disabled="!weddingDate" class="submit-btn">
            Save Wedding Plan
          </button>
        </div>

        <div v-if="hasActiveSelections" class="card contact-card">
          <h3>Selected Vendor Contacts</h3>
          <div v-for="vendor in activeVendorsList" :key="vendor.id" class="contact-profile">
            <p class="vendor-title">{{ vendor.name }} ({{ vendor.type }})</p>
            <p>📞 {{ vendor.phone }}</p>
            <p>✉️ {{ vendor.email }}</p>
            <p>📍 {{ vendor.address }}</p>
            <p class="vendor-commission">Commission: {{ vendor.commissionPercentage }}%</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import axios from 'axios'

const API_BASE_URL = 'https://localhost:7123/api/weddingplanner' 

const vendorsData = reactive({
  restaurant: { items: [], totalPages: 1 },
  pastry: { items: [], totalPages: 1 },
  florist: { items: [], totalPages: 1 },
  hall: { items: [], totalPages: 1 },
  band: { items: [], totalPages: 1 }
})

const weddingDate = ref('')
const selectedRestaurant = ref(null)
const selectedPastry = ref(null)
const selectedFlorist = ref(null)
const selectedHall = ref(null)
const selectedBands = ref([])

const dropdownState = reactive({
  restaurant: false,
  pastry: false,
  florist: false,
  hall: false
})

const filters = reactive({
  restaurant: '',
  pastry: '',
  florist: '',
  hall: '',
  band: ''
})

const pages = reactive({
  restaurant: 1,
  pastry: 1,
  florist: 1,
  hall: 1,
  band: 1
})

const fetchVendors = async (stateKey, categoryName) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/vendors`, {
      params: {
        categoryName: categoryName,
        searchTerm: filters[stateKey],
        page: pages[stateKey],
        pageSize: 3
      }
    })
    vendorsData[stateKey].items = response.data.items
    vendorsData[stateKey].totalPages = response.data.totalPages
  } catch (error) {
    console.error(`Failed to fetch ${categoryName} listings:`, error)
  }
}

onMounted(() => {
  fetchVendors('band', 'Band')
})

const toggleDropdown = (key, categoryName) => {
  const current = dropdownState[key]
  Object.keys(dropdownState).forEach(k => dropdownState[k] = false)
  dropdownState[key] = !current
  
  if (dropdownState[key]) {
    pages[key] = 1
    filters[key] = ''
    fetchVendors(key, categoryName)
  }
}

const changePage = (stateKey, categoryName, direction) => {
  pages[stateKey] += direction
  fetchVendors(stateKey, categoryName)
}

const selectItem = (type, item) => {
  if (type === 'restaurant') selectedRestaurant.value = item
  if (type === 'pastry') selectedPastry.value = item
  if (type === 'florist') selectedFlorist.value = item
  if (type === 'hall') selectedHall.value = item
  dropdownState[type] = false 
}

const checkConflict = (bandA, bandB) => {
  const toMinutes = (timeStr) => {
    if (!timeStr) return 0
    const [hrs, mins] = timeStr.split(':').map(Number)
    return hrs * 60 + mins
  }
  return toMinutes(bandA.startTime) < toMinutes(bandB.endTime) && toMinutes(bandB.startTime) < toMinutes(bandA.endTime)
}

const toggleBand = (band) => {
  const index = selectedBands.value.findIndex(b => b.id === band.id)
  if (index !== -1) {
    selectedBands.value.splice(index, 1)
  } else {
    selectedBands.value = selectedBands.value.filter(b => !checkConflict(b, band))
    selectedBands.value.push(band)
  }
}

const activeVendorsList = computed(() => {
  const list = []
  if (selectedRestaurant.value) list.push({ ...selectedRestaurant.value, type: 'Restaurant' })
  if (selectedPastry.value) list.push({ ...selectedPastry.value, type: 'Pastry' })
  if (selectedFlorist.value) list.push({ ...selectedFlorist.value, type: 'Florist' })
  if (selectedHall.value) list.push({ ...selectedHall.value, type: 'Hall Owner' })
  selectedBands.value.forEach(b => list.push({ ...b, type: 'Band' }))
  return list
})

const hasActiveSelections = computed(() => activeVendorsList.value.length > 0)

const financialSummary = computed(() => {
  let subtotal = 0
  let totalCommission = 0
  activeVendorsList.value.forEach(vendor => {
    subtotal += vendor.price
    totalCommission += vendor.price * (vendor.commissionPercentage / 100)
  })
  return { subtotal, totalCommission, total: subtotal + totalCommission }
})

const submitBooking = async () => {
  const payload = {
    weddingDate: weddingDate.value,
    restaurantId: selectedRestaurant.value?.id || null,
    pastryId: selectedPastry.value?.id || null,
    floristId: selectedFlorist.value?.id || null,
    weddingHallId: selectedHall.value?.id || null,
    bandIds: selectedBands.value.map(b => b.id)
  }

  try {
    const response = await axios.post(`${API_BASE_URL}/weddings`, payload)
    alert(response.data.message || 'Wedding configuration stored completely inside Postgres!')
    
    selectedRestaurant.value = null
    selectedPastry.value = null
    selectedFlorist.value = null
    selectedHall.value = null
    selectedBands.value = []
    weddingDate.value = ''
  } catch (error) {
    console.error('Failed to save wedding config:', error)
    alert(error.response?.data || 'An error occurred while saving to the database.')
  }
}
</script>

<style scoped>
.planner-container { max-width: 960px; margin: 0 auto; padding: 24px; font-family: sans-serif; background-color: #f9fafb; min-height: 100vh; color: #111827; }
.planner-header { text-align: center; margin-bottom: 32px; }
.planner-layout { display: grid; grid-template-columns: 1fr; gap: 24px; }
@media (min-width: 768px) { .planner-layout { grid-template-columns: 2fr 1fr; } }
.form-column { display: flex; flex-direction: column; gap: 24px; }
.card { background-color: #ffffff; padding: 24px; border-radius: 8px; border: 1px solid #e5e7eb; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05); }
.space-y > * + * { margin-top: 20px; }
.form-group { display: flex; flex-direction: column; gap: 6px; }
.label-heading { display: block; font-size: 14px; font-weight: 600; color: #374151; margin-bottom: 6px; }
.input-field, .search-input { width: 100%; padding: 10px; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; box-sizing: border-box; }
.search-input { margin-bottom: 8px; background-color: #f3f4f6; }
.band-search { margin-bottom: 12px; background-color: #fff; }
.help-text { font-size: 12px; color: #6b7280; margin: 0 0 8px 0; }
.custom-dropdown-wrapper { position: relative; }
.dropdown-trigger { display: flex; justify-content: space-between; align-items: center; padding: 12px; background-color: #ffffff; border: 1px solid #d1d5db; border-radius: 6px; font-size: 14px; cursor: pointer; user-select: none; }
.dropdown-trigger:hover { border-color: #9ca3af; }
.arrow-icon { font-size: 10px; color: #6b7280; transition: transform 0.2s ease; }
.arrow-icon.open { transform: rotate(180deg); }
.dropdown-panel { position: absolute; top: 100%; left: 0; right: 0; z-index: 50; margin-top: 4px; background-color: #ffffff; border: 1px solid #d1d5db; border-radius: 6px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); padding: 8px; }
.dropdown-options { max-height: 180px; overflow-y: auto; display: flex; flex-direction: column; gap: 2px; }
.option-item { padding: 10px; font-size: 14px; border-radius: 4px; cursor: pointer; }
.option-item:hover { background-color: #f3f4f6; }
.option-item.selected { background-color: #eff6ff; color: #1e40af; font-weight: 500; }
.no-results { padding: 12px; font-size: 13px; color: #9ca3af; text-align: center; }
.pagination-bar { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; padding-top: 8px; border-top: 1px solid #f3f4f6; font-size: 12px; color: #4b5563; }
.pagination-bar button { background-color: #ffffff; border: 1px solid #d1d5db; padding: 4px 10px; border-radius: 4px; cursor: pointer; }
.pagination-bar button:disabled { background-color: #f3f4f6; color: #9ca3af; cursor: not-allowed; }
.checkbox-list { border: 1px solid #e5e7eb; border-radius: 6px; padding: 12px; max-height: 180px; overflow-y: auto; display: flex; flex-direction: column; gap: 12px; }
.checkbox-item { display: flex; align-items: flex-start; }
.checkbox-input { margin-top: 3px; width: 16px; height: 16px; cursor: pointer; }
.checkbox-label { margin-left: 8px; font-size: 14px; color: #374151; cursor: pointer; }
.schedule-text { display: block; font-size: 12px; color: #6b7280; }
.summary-column { display: flex; flex-direction: column; gap: 24px; }
.sticky-card { position: sticky; top: 24px; }
.summary-column h2 { font-size: 20px; font-weight: 700; margin: 0 0 16px 0; }
.summary-row { display: flex; justify-content: space-between; font-size: 14px; color: #4b5563; }
.date-row { padding-bottom: 16px; border-bottom: 1px solid #f3f4f6; margin-bottom: 16px; }
.financial-details { display: flex; flex-direction: column; gap: 12px; }
.commission-text { color: #16a34a; }
.total-row { border-top: 1px solid #e5e7eb; padding-top: 12px; font-size: 16px; font-weight: 700; color: #111827; }
.submit-btn { width: 100%; margin-top: 24px; background-color: #2563eb; color: #ffffff; padding: 12px; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; }
.submit-btn:hover:not(:disabled) { background-color: #1d4ed8; }
.submit-btn:disabled { background-color: #d1d5db; cursor: not-allowed; }
.contact-card h3 { font-size: 14px; font-weight: 700; margin: 0 0 12px 0; border-bottom: 1px solid #e5e7eb; padding-bottom: 8px; }
.contact-profile { font-size: 12px; color: #4b5563; border-bottom: 1px solid #f3f4f6; padding-bottom: 8px; margin-bottom: 8px; }
.contact-profile:last-child { border: none; margin-bottom: 0; padding-bottom: 0; }
.contact-profile p { margin: 2px 0; }
.vendor-title { font-size: 14px; font-weight: 600; color: #374151; }
.vendor-commission { color: #6b7280; margin-top: 4px; }
</style>