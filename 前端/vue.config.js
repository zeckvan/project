const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  filenameHashing: false,
  transpileDependencies: true,
  lintOnSave: false,
  publicPath: process.env.NODE_ENV === 'production' ? './' : '/',
  outputDir: 'App',
  assetsDir: 'assets',
  productionSourceMap: false, 
  /*
  devServer: {
    proxy: {
      '/api': {
          target: 'http://localhost:80/learnprsservice/api', //請求跨域的目標url
          //target: 'http://skynew.skytek.com.tw/learnprsservice/api', //請求跨域的目標url
          secure: false, //false為http訪問，true為https訪問
          changeOrigin: true, //是否進行跨域處理
          pathRewrite: {
              '^/api/': '' //重寫請求目標的url
          },
          ws:true
      }
    }
  }
  */
})


