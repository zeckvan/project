<template>
  <div>
    <PubQuery2 @get-yms="getYms"></PubQuery2>
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
import PubQuery2 from '@/components/pub/pub_query2.vue'

const options = ['選取'];
export default {
  props: {
    url: {
      type: String,
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
    getYms(yms){
      this.year = yms.year
      this.sms = yms.sms
      this.genDateTimeList()
    },
    genDateTimeList: function () {
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.url}`
      
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
              sms: _self.sms
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
      const apiurl = `${this.$apiroot}${this.$props.url}`

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
    PubQuery2
  },
  beforeDestroy(){

  },
  mounted() {

  },
  beforeMount() {
    
  },
  computed: {
    
  }
}
</script>

<style>

</style>
