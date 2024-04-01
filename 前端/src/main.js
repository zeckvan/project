import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import locale from 'element-ui/lib/locale/lang/zh-TW'
import { Message } from "element-ui";
//import axios from 'axios'
//import VueAxios from 'vue-axios'
//import moment from 'vue-moment'

//Vue.use(VueAxios, axios)
Vue.use(ElementUI, { locale })
Vue.config.productionTip = false
//axios.defaults.withCredentials = true;
new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
