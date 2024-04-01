<template>
  <div>

    <!--承辦人員-->
    <el-row type="flex" justify="space-around" v-if="$props.urlObject!=undefined">
      <el-col :span="6">
        學年期：
        <el-select v-model="ymsComputed" @change="getClsStd" :placeholder="$props.urlObject.ymsOption.length > 0 ? '請選擇':'查無學年期，請先匯入資料！'">
          <el-option v-for="yms in $props.urlObject.ymsOption" :key="yms" :label="$props.urlObject.ymsString(yms)" :value="yms">
          </el-option>
        </el-select>
      </el-col>
      <el-col :span="6">
        <el-select v-if="clsOption.length > 0" v-model="cls" @change="changeStdOption($event)" placeholder="請選擇班級">
          <el-option key="" label="" value=""></el-option>
          <el-option v-for="item in clsOption" :key="item" :label="item" :value="item">
          </el-option>
        </el-select>
      </el-col>
      <el-col :span="6">
        <el-select v-if="stdOption.length > 0" v-model="std" @change="changeStd" placeholder="請選擇學生">
          <el-option key="" label="" value=""></el-option>
          <el-option v-for="item in stdOption" :key="item.val" :label="item.name" :value="item.val">
          </el-option>
        </el-select>
      </el-col>
      <el-col :span="6">
        <el-button v-if="stdOption.length > 0" type="primary" @click="postStdList">查詢</el-button>
      </el-col>
    </el-row>

    <!--學生-->
    <el-row type="flex" justify="space-around" v-if="$props.std_urlObject != undefined">
      <el-col :span="6">
        學年期：
        <el-select v-model="ymsComputed" @change="initStdDataObject" :placeholder="ymsOption.length > 0 ? '請選擇':'查無學年期，尚未有中央資料庫資料！'">
          <el-option v-for="yms in ymsOption" :key="yms" :label="ymsString(yms)" :value="yms">
          </el-option>
        </el-select>
      </el-col>
      <el-col :span="6">
        <el-button type="primary" v-if="year!='' && sms!=''"  @click="postStdList">查詢</el-button>
      </el-col>
    </el-row>

    <!--共用-->
    <el-row type="flex" justify="space-around">
      <el-col>
        <el-card class="box-card">
            <el-tabs v-if="stdDataObject.length > 0" v-model="activeName" @tab-click="">
              <el-tab-pane v-for="(item, index) in stdDataObject" :key="index" :label="item['名稱']" :name="index+''">
                <stdDetail v-if="$props.urlObject!=undefined" :stdObject="item"></stdDetail>
                <stdDetail
                  v-else-if="$props.std_urlObject!=undefined"
                  :stdObject="item"
                  :std_urlObject="$props.std_urlObject"
                  @std-CheckData="stdCheckData"
                ></stdDetail>
              </el-tab-pane>
            </el-tabs>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script type="module">
import stdDetail from "@/components/centerDB/centerDB_query_std_detail.vue"

export default {
  props: {
    urlObject: {
      type: Object,
    },
    std_urlObject: {
      type: Object,
    }
  },
  data() {
    return {
      //承辦人員用
      stdsOption: [],
      clsOption: [],
      stdOption: [],
      cls: "",
      //學生用
      ymsOption: [],
      //共用
      std: "",
      activeName: '0',
      year: "",
      sms: "",
      stdDataObject: []
    }
  },
  methods: {
    //承辦人員用
    changeStdOption(even) {
      let index = this.clsOption.indexOf(even)
      if (index > -1) {
        this.stdOption = this.stdsOption[index]
        this.std = ""
        this.initStdDataObject()
      }
    },
    changeStd() {
      this.initStdDataObject()
    },
    getClsStd() {
      this.cls = ""
      this.std = ""
      this.initStdDataObject()

      const _self = this
      const apiurl = `${_self.$apiroot}${_self.$props.urlObject.QueryStd}`

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        apiurl,
        {
          params: {
            year: _self.year,
            sms: _self.sms
          },
          headers:{'SkyGet':_self.$token}
        }
      )
        .then((res) => {
          if (res.data.status == 'Y') {
            _self.clsOption = res.data.dataset.cls
            _self.stdsOption = res.data.dataset.std
          } else {
            _self.clsOption = []
            _self.stdsOption = []
            _self.$message.error('查無學生資料，請先匯入！')
          }
        })
        .catch((error) => {
          _self.clsOption = []
          _self.stdsOption = []
          _self.$message({
            message: '系統發生錯誤' + error,
            type: 'error',
            duration: 0,
            showClose: true,
          })
        })
        .finally(() => {
          _self.stdOption = []
          loading.close()
        })
    },
    //學生用
    ymsString: function(val) {
          let y = ""
          let s = ""
          if (val.length > 3) {
              y = val.substr(0, 3)
              s = val.substr(3, 1)
          }
          else {
              y = val.substr(0, 2)
              s = val.substr(2, 1)
          }
          return `${y}學年${s}學期`
    },
    getYms() {
      const _self = this
      const apiurl = `${_self.$apiroot}${_self.$props.std_urlObject.GetStdYms}`

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        apiurl,
        {
          params:{
            arg: _self.std
          },
          headers:{'SkyGet':_self.$token}
        }
      )
      .then((res) => {
          if (res.data.status == 'Y') {
              _self.ymsOption = res.data.dataset
          }
      })
      .catch((error) => {
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    stdCheckData(kind){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.StdCheckData}`

      //送出
      const loading = _self.$loading({
        lock: true,
        text: '設定中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      _self.$http({
        url:apiurl,
        method:"put",
        data:{
          "year_id": _self.year,
          "sms_id": _self.sms,
          "kind": kind,
          "std": _self.std
        },
        headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        if (res.data.status == 'Y'){
          _self.$message.success(res.data.message)
        }else{
          _self.$message.error(res.data.message)
        }
      })
      .catch((error) => {
          _self.$message({
            message: '系統發生錯誤'+error,
            type: 'error',
            duration:0,
            showClose: true,
          })
        })
      .finally(() => loading.close())
    },
    //共用
    postStdList() {
      const _self = this
      let type_id = ''

      if(_self.std==""){
        _self.$message.error('請選擇學生！')
        return
      }

      let apiurl = ''

      if(_self.$props.urlObject != undefined){
        apiurl = `${_self.$apiroot}${_self.$props.urlObject.QueryStd}`
        type_id = '1'
      }
      else if(_self.$props.std_urlObject != undefined){
        apiurl = `${_self.$apiroot}${_self.$props.std_urlObject.QueryStd}`
        type_id = '2'
      }

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      let formData = new FormData()
      formData.append("year", _self.year)
      formData.append("sms", _self.sms)
      formData.append("std", _self.std)
      formData.append("token", _self.$token)
      formData.append("type", type_id)
      this.axios({
          method: 'post',
          url: apiurl,
          data: formData,
          headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        if (res.data.status == 'Y') {
          _self.stdDataObject = res.data.dataset
        } else {
          _self.stdDataObject = []
          _self.$message.error('查無學生資料，請先匯入！')
        }
      })
      .catch((error) => {
        _self.stdDataObject = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    initStdDataObject(){
      this.activeName = '0'
      this.stdDataObject = []
    },
  },
  components: {
    //共用，但會因是承辦人員或學生傳入不同的porps
    stdDetail
  },
  mounted() {
  },
  async beforeMount() {
    //學生
    if( this.$props.std_urlObject != undefined){
      this.std = this.$token  //這邊到時候要改成VUEX的學生資料
      await this.getYms()
    }
  },
  computed: {
    //共用
    ymsComputed: {
      get() {
        return this.year + this.sms + ""
      },
      set(val) {
        if (val.length > 3) {
          this.year = val.substr(0, 3)
          this.sms = val.substr(3, 1)
        }
        else {
          this.year = val.substr(0, 2)
          this.sms = val.substr(2, 1)
        }
      }
    },
  }
}
</script>

<style></style>
