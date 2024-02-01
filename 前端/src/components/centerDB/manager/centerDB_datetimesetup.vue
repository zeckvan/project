<template>
  <div>
    <el-row>
      <el-col :span="1">&nbsp;</el-col>
      <el-col :span="23" style="text-align: left;">
      學年期：
      <el-select v-model="yms" :placeholder="$props.urlObject.ymsOption.length > 0 ? '請選擇':'查無學年期，請先匯入資料！'">
        <el-option v-for="yms in $props.urlObject.ymsOption"
          :key="yms"
          :label="$props.urlObject.ymsString(yms)"
          :value="yms"
        >
        </el-option>
      </el-select>
      <el-button type="primary" @click="genDateTimeList">查詢</el-button>
      </el-col>
    </el-row>
    

    <el-table :data="tableData" stripe style="width: 100%">

      <el-table-column align="center" prop="check" >
        <template slot-scope="scope">
            <el-checkbox label="選取" @change="selectItem($event,scope.$index)"></el-checkbox>
        </template>
      </el-table-column>

      <el-table-column label="項目" prop="kind"></el-table-column>
      <el-table-column label="開始時間" prop="s_dt" >
        <template slot-scope="scope">
          <el-date-picker
            v-model="tableData[scope.$index].s_dt"
            type="datetime"
            placeholder="選擇日期時間"
            value-format="yyyy-MM-dd HH:mm"
          >
          </el-date-picker>
        </template>
      </el-table-column>

      <el-table-column label="結束時間" prop="e_dt">
        <template slot-scope="scope">
          <el-date-picker
            v-model="tableData[scope.$index].e_dt"
            type="datetime"
            placeholder="選擇日期時間"
            value-format="yyyy-MM-dd HH:mm"
          >
          </el-date-picker>
        </template>
      </el-table-column>

      <el-table-column label="壓縮檔來源" prop="zip_name"></el-table-column>
    </el-table>
    <el-button type="primary" @click="upsert">確定設定收訖時間</el-button>
  </div>
</template>

<script type="module">

export default {
  props: {
    urlObject: {
      type: Object,
    },
  },
  data() {
    return {
      year:"",
      sms:"",
      tableData: []
    }
  },
  methods: {
    selectItem(e, index){
      this.tableData[index].check = e ? 1 : 0
    },
    genDateTimeList() {
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.DateSetup}`
      
      if(_self.year=="" || _self.sms=="")
      {
        _self.$message.error('請選擇學年期')
        return
      }

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        apiurl, {
          params: {
              year: _self.year,
              sms: _self.sms,
              token:_self.$token
          },
          headers:{'SkyGet':_self.$token}
        }
      )
      .then((res) => {
        if (res.data.status == 'Y') {
          _self.tableData = res.data.dataset
        } else {
          _self.tableData = []
          _self.$message.error('查無資料，請先匯入')
        }
      })
      .catch((error) => {
          _self.tableData = []
          _self.$message({
            message: '系統發生錯誤'+error,
            type: 'error',
            duration:0,
            showClose: true,
          })
        })
      .finally(() => loading.close())
    },
    upsert() {
      let _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.DateSetup}`

      if(_self.tableData.length == 0)
      {
        return
      }
      //沒選擇不可送出
      let isSelected = false
      _self.tableData.forEach(element => {
        if(element.check == 1)
        {
          isSelected = true
        }
        else{
          return
        }
      });
      if(!isSelected)
      {
        _self.$message({
          message: '未選擇設定項目',
          type: 'error',
          duration:0,
          showClose: true,
        })
        return
      }
      //時間判斷
      for(let i=0; i < _self.tableData.length; i++)
      {
        if(_self.tableData[i].check == 1){
          if(_self.tableData[i].s_dt == "" || _self.tableData[i].e_dt == "")
          {
            _self.$message({
              message: `${_self.tableData[i].kind}：請設定『開始』及『結束』時間`,
              type: 'error',
              duration:0,
              showClose: true,
            })
            return
          }
         
          let sdt = new Date(_self.tableData[i].s_dt)
          let edt = new Date(_self.tableData[i].e_dt)

          if(sdt > edt)
          {
            _self.$message({
              message: `${_self.tableData[i].kind}：時間錯誤，『開始時間』大於『結束時間』`,
              type: 'error',
              duration:0,
              showClose: true,
            })
            return
          }
        }
        _self.tableData[i].token = _self.$token
      }
      //送出
      const loading = _self.$loading({
        lock: true,
        text: '設定中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      _self.$http({
        url:apiurl,
        method:"post",
        data:_self.tableData,
        headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        if (res.data.status == 'Y'){
          _self.$message.success(res.data.message)
          _self.tableData = []
          _self.genDateTimeList()
        }else{
          _self.$message.error(res.data.message)
        }
      })
      .catch((error) => {
          _self.tableData = []
          _self.$message({
            message: '系統發生錯誤'+error,
            type: 'error',
            duration:0,
            showClose: true,
          })
        })
      .finally(() => loading.close())
    },
  },
  components: {
  },
  beforeDestroy(){
  },
  mounted() {
  },
  beforeMount() {
  },
  computed: {
    yms: {
      get(){
        return this.year + this.sms + ""
      },
      set(val){
        if(val.length > 3)
        {
          this.year = val.substr(0,3)
          this.sms = val.substr(3,1)
        }
        else{
          this.year = val.substr(0,2)
          this.sms = val.substr(2,1)
        }
      }
    },
  }
}
</script>

<style>

</style>
