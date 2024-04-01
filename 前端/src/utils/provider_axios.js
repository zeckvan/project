import Vue from 'vue'
import axios from 'axios'
import { Loading,Message } from 'element-ui';
//Vue.config.productionTip = false
//axios.defaults.withCredentials = true;

var loading = null
let loadingCount = 0
let cookieObj = {};
let cookieAry = document.cookie.split(';');
let cookie;
parseCookie()
function parseCookie() {
  for (let i=0, l=cookieAry.length; i<l; ++i) {//
      if(cookieAry[i].includes('=')){
        cookie = cookieAry[i].split('=');
        cookieObj[cookie[0]] = cookie[1];
      }
  }
}

//Vue.prototype.$token = cookieObj[' X-Token']
Vue.prototype.$token = '52a6bb09'

const axiosInstance = axios.create({
    baseURL: window.apiConfig.api,
    headers:{'SkyGet':Vue.prototype.$token}
    //timeout: 1000,
  });

axiosInstance.interceptors.request.use(
    request=> {        
        if(loadingCount === 0)
        {
          loading = Loading.service({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
            });  
        }
        loadingCount++
        return request;
    },
     error=> {
        loadingCount--
        if(loadingCount <= 0 && loading){
          loadingCount = 0
          loading.close()
        }
        return Promise.reject(error);
    }
);
  
axiosInstance.interceptors.response.use(
  response => {
    loadingCount--
    if(loadingCount <= 0 && loading){
      loadingCount = 0
      loading.close()
    }

    if (response.status === 200) {              
        return Promise.resolve(response);               
    } 
    else {        
        return Promise.reject(response);        
    }
         
  },
  error => {
    if (error && error.response) {
        switch (error.response.status) {
          case 400:
            error.message = 'Request Error!'
            break
          case 401:
            error.message = '無權限重新登入系統'
            break
          case 403:
            error.message = 'Access denied!'
            break
          case 404:
            // 自動帶入 request 地址的寫法
            //error.message = `Address not exist: ${error.response.config.url}`
            error.message = `Address not exist:`
            break
          case 408:
            error.message = 'Request timeout!'
            break
          case 500:
            error.message = 'Server inside error!'
            break
          case 501:
            error.message = 'Service not allowed!'
            break
          case 502:
            error.message = 'Bad gateway!'
            break
          case 503:
            error.message = 'No service!'
            break
          case 504:
            error.message = 'Gateway timeout!'
            break
          case 505:
            error.message = 'http version not supported!'
            break
          default:
            break
        }
      }

    loadingCount--
    if(loadingCount <= 0 && loading){
      loadingCount = 0
      loading.close()
    }
    Message({
        message: error.message,
        type: 'error',
        duration: 5 * 1000
    })
    return Promise.reject(error.response.data)
  }
);

export const apiHelper = axiosInstance