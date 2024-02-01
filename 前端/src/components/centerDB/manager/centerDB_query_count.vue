<template>
  <div>
    <el-row type="flex" justify="space-around">
      <el-col :span="6">
        學年期：
        <el-select v-model="ymsComputed" @change="GetCls" :placeholder="$props.urlObject.ymsOption.length > 0 ? '請選擇':'查無學年期，請先匯入資料！'">
          <el-option v-for="yms in $props.urlObject.ymsOption" :key="yms" :label="$props.urlObject.ymsString(yms)" :value="yms">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
        <el-select v-model="cls" placeholder="請選擇">
          <el-option v-for="cls in clsOption" :key="cls" :label="cls" :value="cls">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
        <el-button type="primary" @click="GetQueryCount">查詢</el-button>
      </el-col>
    </el-row>
    <el-row>
      <el-col>
        <el-descriptions class="margin-top" title="統計" :column="3" border>
          <el-descriptions-item v-for="item in countData">
            <template slot="label">
              <i class="el-icon-user"></i>
              {{item.kind}}
            </template>
            {{`${item.x_check}已確認/共${item.x_cnt}人`}}
          </el-descriptions-item>
        </el-descriptions>
      </el-col>
    </el-row>
    <el-row v-if="tableData.length>0">
      <el-col>
          <el-table
            :data="tableData"
            height="65vh"
            style="width: 100%">
            <el-table-column
              v-for="t in tableHead"
              :prop="t"
              :label="t"
              :width="setwidth(t)"
            >
            <template slot-scope="scope">
                <span :style="{color:scope.row[t]=='N'?'red':''}">{{changeCheck(scope.row[t])}}</span>
            </template>
            </el-table-column>
          </el-table>
      </el-col>
    </el-row>
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
      clsOption: [],
      countData:[],
      tableHead:[],
      tableData:[],
      year: "",
      sms: "",
      cls:"",
    }
  },
  methods: {
    GetCls(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.GetCls}`

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        `${apiurl}`,
          {
            params:{ 
              year: _self.year,
              sms: _self.sms,
              token:_self.$token
            },
            headers:{'SkyGet':_self.$token}
          }
      )
      .then((res) => {
        if (res.data.status == 'Y') {
          let all = ['All']
          _self.clsOption = all.concat(res.data.dataset);
        } else {
          _self.clsOption = []
          _self.$message.error('查無資料，請先匯入！')
        }
      })
      .catch((error) => {
        _self.clsOption = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    GetQueryCount(){
      
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.QueryCount}`

      if(_self.cls == ""){
        _self.$message.error('請選擇班級')
        return
      }

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        `${apiurl}`,
          {
            params:{ 
              year: _self.year,
              sms: _self.sms,
              cls: _self.cls,
              token:_self.$token
            },
            headers:{'SkyGet':_self.$token}
          }
      )
      .then((res) => {
        if (res.data.status == 'Y') {
          _self.tableHead = res.data.dataset.head
          _self.tableData = res.data.dataset.datas
          _self.countData = res.data.dataset.counts
        } else {
          _self.countData = []
          _self.tableHead = []
          _self.tableData = []
          _self.$message.error('查無資料，請先匯入！')
        }
      })
      .catch((error) => {
        _self.countData = []
        _self.tableHead = []
        _self.tableData = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    changeCheck(val){
      if(val=="N"){
        return "待確認"
      }
      else if(val == "Y"){
        return ""
      }
      else{
        return val
      }
    },
    setwidth(val){
      const item = ['座號','學號','姓名']
      if(item.indexOf(val)>-1){
        return 70
      }
      return ""
    }
  },
  components: {
  },
  mounted() {
  },
  beforeMount() {
  },
  computed: {
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
    }
  }
}
</script>

<style></style>
