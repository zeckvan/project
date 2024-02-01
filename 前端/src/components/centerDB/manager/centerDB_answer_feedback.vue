<template>
  <div>
    <el-row type="flex" justify="space-around">
      <el-col :span="6">
        學年期：
        <el-select v-model="ymsComputed" @change="GetFeedbackKindCls" :placeholder="$props.urlObject.ymsOption.length > 0 ? '請選擇':'查無學年期，請先匯入資料！'">
          <el-option v-for="yms in $props.urlObject.ymsOption" :key="yms" :label="$props.urlObject.ymsString(yms)" :value="yms">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
        <el-select v-if="kindOption.length > 0" v-model="kind" @change="changeClsOption" placeholder="請選擇">
          <el-option v-for="item in kindOption" :key="item" :label="item" :value="item">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
        <el-select v-if="clsOption.length > 0" v-model="cls" @change="init_sRow_eRow" placeholder="請選擇">
          <el-option v-for="item in clsOption" :key="item" :label="item" :value="item">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
        <el-button v-if="kindOption.length > 0" type="primary" @click="getFreeback">查詢</el-button>
      </el-col>
    </el-row>
    <el-row v-if="tableData.length>0">
      <el-col>
          <el-table
            :data="tableData"
            height="50vh"
            style="width: 100%">
            <el-table-column
              label=""
              width="100"
            >
            <template slot-scope="item">
              <el-button type="warning" size="mini" @click="tablehandleEdit(item.row, item.$index)">答覆</el-button>
            </template>
            </el-table-column>
            <el-table-column
              prop="ser_id"
              label="單號"
              sortable
            >
            </el-table-column>
            <el-table-column
              v-if="isShowKind"
              prop="kind"
              label="項目"
              sortable
            >
            </el-table-column>
            <el-table-column
              v-if="isShowCls"
              prop="cls"
              label="類別"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="cls_abr"
              label="班級"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="std_name"
              label="姓名"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="sit_num"
              label="座號"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="name"
              label="錯誤別"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="std_feedback"
              label="問題描述"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="answer"
              label="答覆"
              sortable
            >
            </el-table-column>
            <el-table-column
              prop="createDt"
              label="回報時間"
              sortable
            >
            </el-table-column>
          </el-table>
          
          <el-pagination
            @current-change="handleCurrentChange"
            :current-page.sync="currentPage"
            :page-size="pageSize"
            layout="total, prev, pager, next"
            :total="total">
          </el-pagination>

          <el-dialog
            title="答覆"
            :visible.sync="dialogVisible"
            :close-on-click-modal="false"
            :close-on-press-escape="false"
            :before-close="dialoghandleClose"
          >
          <el-form>
            <el-form-item label="學生問題">
              <el-input type="textarea" v-model="feedback" disabled></el-input>
            </el-form-item>
            <el-form-item label="答覆">
              <el-input type="textarea" v-model="answer"></el-input>
            </el-form-item>
          </el-form>
          <span slot="footer" class="dialog-footer">
            <el-button @click="dialogVisible = false">取 消</el-button>
            <el-button type="primary" @click="dialoghandleSave">保 存</el-button>
          </span>
          </el-dialog>
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
      dialogVisible: false,
      kindclsOption: [],
      tableData:[],
      tableDataIndex:-1,
      total:0,
      pageSize:10,
      currentPage:1,
      sRow:1,
      eRow:10,
      year: "",
      sms: "",
      kind:"",
      cls:"",
      ser_id:0,
      feedback:"",
      answer:""
    }
  },
  methods: {
    GetFeedbackKindCls(){
      this.kind = 'All'
      this.cls = 'All'

      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.FeedBackKindCls}`

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get(
        `${apiurl}/${_self.year}/${_self.sms}`,{headers:{'SkyGet':_self.$token}}
      )
      .then((res) => {
        if (res.data.status == 'Y') {
          _self.kindclsOption = res.data.dataset
        } else {
          _self.kindclsOption = []
          _self.$message.error('查無資料，請先匯入！')
        }
      })
      .catch((error) => {
        _self.kindclsOption = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    changeClsOption(){
      this.init_sRow_eRow()
      this.cls = "All"
    },
    getFreeback(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.Feedback}`

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
            year_id: _self.year,
            sms_id: _self.sms,
            kind: _self.kind,
            cls: _self.cls,
            sRow: _self.sRow,
            eRow: _self.eRow,
            token:_self.$token
          },
          headers:{'SkyGet':_self.$token}
        }
      )
      .then((res) => {
        if (res.data.status == 'Y') {
          if(res.data.dataset[0]){
            _self.total = res.data.dataset[0].total
          }
          _self.tableData = res.data.dataset
        } else {
          _self.tableData = []
          _self.$message.error('查無資料，請先匯入！')
        }
      })
      .catch((error) => {
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
    handleCurrentChange(val) {
      let _self = this
      _self.changePageRange(val)
      _self.getFreeback()
    },
    changePageRange(val){
      let range = this.pageSize * val
      this.sRow = range - this.pageSize +1
      this.eRow = range
    },
    init_sRow_eRow(){
      this.sRow = 1
      this.eRow = 10
      this.currentPage = 1
    },
    tablehandleEdit(row, index){
      this.ser_id = row.ser_id
      this.feedback = row.std_feedback
      this.answer = row.answer
      this.dialogVisible = true
      this.tableDataIndex = index
    },
    dialoghandleClose(){
      this.ser_id = 0
      this.feedback = ""
      this.answer = ""
      this.tableDataIndex = -1
    },
    dialoghandleSave(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.urlObject.Feedback}`

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
          "ser_id": _self.ser_id,
          "answer": _self.answer,
          "token":_self.$token
        },
        headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        if (res.data.status == 'Y'){
          _self.$message.success(res.data.message)

          _self.tableData[_self.tableDataIndex].answer = _self.answer
          _self.dialogVisible = false
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
    },
    kindOption(){
      let _self = this
      if(_self.kindclsOption.length > 0){
        let kind = ['All']
        _self.kindclsOption.forEach((val, index, array) => {
          if(kind.indexOf(val.kind) == -1){ kind.push(val.kind) }
        })
        return kind
      }
      return []
    },
    clsOption(){
      let _self = this
      if(_self.kindOption.length > 0){
        let cls = ['All']
        _self.kindclsOption
        .filter((val) => {
          if(_self.kind == "All"){
            return val
          }
          else if(_self.kind == val.kind){
            return val
          }
        })
        .forEach((val, index, array) => {
          if(cls.indexOf(val.cls) == -1){ cls.push(val.cls) }
        })
        return cls
      }
      return []
    },
    isShowKind(){
      let _self = this
      let kind = []
      if(_self.tableData.length > 0){
        _self.tableData.forEach((val, index, array) => {
          if(kind.indexOf(val.kind) == -1){ kind.push(val.kind) }
        })
      }

      return kind.length > 1 ? true : false
    },
    isShowCls(){
      let _self = this
      let cls = []
      if(_self.tableData.length > 0){
        _self.tableData.forEach((val, index, array) => {
          if(cls.indexOf(val.cls) == -1){ cls.push(val.cls) }
        })
      }

      return cls.length > 1 ? true : false
    }
  }
}
</script>

<style></style>
