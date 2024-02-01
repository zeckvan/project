<template>
  <div>
    <el-row type="flex" justify="space-around">
      <el-col :span="6">
        學年期：
        <el-select v-model="ymsComputed" @change="queryData" :placeholder="setting.ymsOption.length > 0 ? '請選擇':'查無學年期，尚未有中央資料庫資料！'">
          <el-option v-for="yms in setting.ymsOption" :key="yms" :label="ymsString(yms)" :value="yms">
          </el-option>
        </el-select>
      </el-col>

      <el-col :span="6">
      </el-col>
    </el-row>
    <el-divider></el-divider>

    <el-row style="text-align: right;">
      <el-button type="success"
        v-if="year != '' && sms != '' && setting.openKind.length > 0"
        @click="tablehandleAdd"
      >新增</el-button>
    </el-row>
    <el-row v-if="year != '' && sms != ''">
      <el-col>
          <el-table
            :data="tableData"
            height="50vh"
            style="width: 100%">
            <el-table-column
              prop="ser_id"
              label="單號"
              width="70"
            >
            </el-table-column>
            <el-table-column
              prop="kind"
              label="項目"
            >
            </el-table-column>
            <el-table-column
              prop="cls"
              label="類別"
            >
            </el-table-column>
            <el-table-column
              prop="name"
              label="錯誤別"
              width="80"
            >
            </el-table-column>
            <el-table-column
              prop="std_feedback"
              label="問題"
            >
            </el-table-column>
            <el-table-column
              prop="answer"
              label="答覆"
            >
            </el-table-column>
            <el-table-column
              label=""
              width="150"
            >
            <template slot-scope="item" v-if="setting.openKind.indexOf(item.row.kind) > -1">
              <el-button-group>
                <el-button type="warning" size="mini" @click="tablehandleEdit(item.row, item.$index)">編輯</el-button>
                <el-popconfirm
                  confirm-button-text='確定'
                  cancel-button-text='取消'
                  icon="el-icon-info"
                  icon-color="red"
                  title="確定刪除？"
                  @confirm="tablehandleDel(item.row, item.$index)"
                >
                  <el-button type="danger" size="mini" slot="reference" >刪除</el-button>
                </el-popconfirm>
              </el-button-group>
            </template>
            </el-table-column>
          </el-table>
          
          <el-pagination
            @current-change="handleCurrentChange"
            :current-page.sync="currentPage"
            :page-size="setting.pageSize"
            layout="total, prev, pager, next"
            :total="total">
          </el-pagination>

          <el-dialog
            :visible.sync="setting.dialogVisible"
            :close-on-click-modal="false"
            :close-on-press-escape="false"
            :show-close="false"
            :before-close="dialoghandleClose"
          >
          <el-form :model="form" :rules="setting.rules" ref="myForm">
            <el-form-item label="項目" prop="kind">
              <el-select v-model="form.kind" @change="form.cls=null" placeholder="請選擇">
                <el-option v-for="kind in kindOption" :label="kind" :key="kind" :value="kind"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="類別" prop="cls">
              <el-select v-model="form.cls" placeholder="請選擇">
                <el-option v-for="cls in clsOption" :label="cls" :value="cls" :key="cls"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="錯誤別" prop="error_code">
              <el-select v-model="form.error_code" placeholder="請選擇">
                <el-option v-for="e in setting.errorOption" :label="e.name" :value="e.id" :key="e.id"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item  label="問題回饋" prop="std_feedback">
              <el-input type="textarea" v-model="form.std_feedback"></el-input>
            </el-form-item>
          </el-form>
          <span slot="footer" class="dialog-footer">
            <el-button @click="dialoghandleClose">取 消</el-button>
            <el-button v-if="tableDataIndex==-1" type="primary" @click="dialoghandleAdd">新 增</el-button>
            <el-button v-else type="primary" @click="dialoghandleSave">保 存</el-button>
          </span>
          </el-dialog>
          
      </el-col>
    </el-row>
  </div>
</template>

<script type="module">

export default {
  props: {
    std_urlObject: {
      type: Object,
    },
  },
  data() {
    return {
      setting:{
        ymsOption: [],
        openKind:[],
        kindclsOption: [],
        errorOption:[],
        dialogVisible: false,
        pageSize:10,
        rules: {
          kind: [
            {required: true, message: '請選擇', trigger: 'change' },
          ],
          cls: [
            {required: true, message: '請選擇', trigger: 'change' }
          ],
          error_code: [
            { type: 'number', required: true, message: '請選擇', trigger: 'change' }
          ]
        }
      },
      tableData:[],
      tableDataIndex:-1,
      total:0,
      currentPage:1,
      sRow:1,
      eRow:10,
      form:{
        year_id:null,
        sms_id:null,
        ser_id:0,
        kind:null,
        cls:null,
        token:null,
        error_code:null,
        std_feedback:null,
      },
      year: "",
      sms: "",
      std: ""
    }
  },
  methods: {
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
      const apiurl = `${_self.$apiroot}${_self.$props.std_urlObject.GetYmsStd}`

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
              _self.setting.ymsOption = res.data.dataset
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
    GetFeedbackKindCls(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.FeedBackKindCls}`

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
          _self.setting.kindclsOption = res.data.dataset
        } else {
          _self.setting.kindclsOption = []
          _self.$message.error('查無資料！')
        }
      })
      .catch((error) => {
        _self.setting.kindclsOption = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    GetFeedbackOpenKind(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.FeedBackOpenKind}`

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
          _self.setting.openKind = res.data.dataset
        } else {
          _self.setting.openKind = []
          _self.$message.error('查無資料，尚未開放！')
        }
      })
      .catch((error) => {
        _self.setting.openKind = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    GetFeedBackErrorCode(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.FeedBackErrorCode}`

      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      this.axios.get( apiurl, {
        headers:{'SkyGet':_self.$token}
        } )
      .then((res) => {
        if (res.data.status == 'Y') {
          _self.setting.errorOption = res.data.dataset
        } else {
          _self.setting.errorOption = []
          _self.$message.error('查無資料！')
        }
      })
      .catch((error) => {
        _self.setting.errorOption = []
        _self.$message({
          message: '系統發生錯誤' + error,
          type: 'error',
          duration: 0,
          showClose: true,
        })
      })
      .finally(() => loading.close())
    },
    getFreeback(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.StdFeedback}`

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
    async queryData(){
      this.GetFeedbackKindCls()
      this.GetFeedbackOpenKind()
      this.GetFeedBackErrorCode()
      await this.getFreeback()
    },
    async handleCurrentChange(val) {
      let _self = this
      _self.changePageRange(val)
      await _self.getFreeback()
      if(_self.total < 10){
        init_sRow_eRow()
      }
    },
    changePageRange(val){
      let range = this.setting.pageSize * val
      this.sRow = range - this.setting.pageSize +1
      this.eRow = range
    },
    init_sRow_eRow(){
      this.sRow = 1
      this.eRow = 10
      this.currentPage = 1
    },
    tablehandleAdd(){
      let _self = this
      Object.keys(_self.form).forEach((key)=>{
        _self.form[key] = null
      })

      this.setting.dialogVisible = true
    },
    tablehandleEdit(row, index){
      let _self = this
      Object.keys(_self.form).forEach((key)=>{
        _self.form[key] = row[key]
      })

      this.tableDataIndex = index
      this.setting.dialogVisible = true
    },
    tablehandleDel(row, index){
      const _self = this
      const apiurl = `${_self.$apiroot}${this.$props.std_urlObject.StdFeedback}`

      //送出
      const loading = _self.$loading({
        lock: true,
        text: '設定中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      _self.form.year_id = _self.year
      _self.form.sms_id = _self.sms
      _self.form.token = _self.std
      _self.form.ser_id = row.ser_id
      _self.form.kind = row.kind

      _self.$http({
        url:apiurl,
        method:"delete",
        data: _self.form,
        headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        if (res.data.status == 'Y'){
          _self.$message.success(res.data.message)

          _self.init_sRow_eRow()
          _self.getFreeback()
          _self.dialoghandleClose()
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
    dialoghandleClose(){
      let _self = this
      Object.keys(_self.form).forEach((key)=>{
        _self.form[key] = null
      })
      _self.form.ser_id = 0
      _self.tableDataIndex = -1
      _self.setting.dialogVisible = false
    },
    dialoghandleAdd(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.StdFeedback}`

      _self.$refs['myForm'].validate((valid) => {
        if (valid) {
          //送出
          const loading = _self.$loading({
            lock: true,
            text: '設定中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          _self.form.year_id = _self.year
          _self.form.sms_id = _self.sms
          _self.form.token = _self.std
          _self.form.ser_id = 0

          _self.$http({
            url:apiurl,
            method:"post",
            data: _self.form,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
            if (res.data.status == 'Y'){
              _self.$message.success(res.data.message)

              _self.init_sRow_eRow()
              _self.getFreeback()
              _self.dialoghandleClose()
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
      });
    },
    dialoghandleSave(){
      const _self = this
      const apiurl = `${this.$apiroot}${this.$props.std_urlObject.StdFeedback}`

      _self.$refs['myForm'].validate((valid) => {
        if (valid) {
          //送出
          const loading = _self.$loading({
            lock: true,
            text: '設定中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });
          
          _self.form.token = _self.$token
          _self.$http({
            url:apiurl,
            method:"put",
            data: _self.form,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
            if (res.data.status == 'Y'){
              _self.$message.success(res.data.message)

              Object.keys(_self.form).forEach((key)=>{
                _self.tableData[_self.tableDataIndex][key] = _self.form[key]
              })

              _self.dialoghandleClose()
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
      });
    }
  },
  components: {
  },
  mounted() {
  },
  async beforeMount() {
    this.std = this.$token
    await this.getYms()
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
      if(_self.setting.kindclsOption.length > 0){
        let kind = []
        _self.setting.kindclsOption.forEach((val, index, array) => {
          if(kind.indexOf(val.kind) == -1 && _self.setting.openKind.indexOf(val.kind) > -1){ kind.push(val.kind) }
        })
        return kind
      }
      return []
    },
    clsOption(){
      let _self = this
      if(_self.kindOption.length > 0){
        let cls = []
        _self.setting.kindclsOption
        .filter((val) => {
          if(_self.form.kind == val.kind){
            return val
          }
        })
        .forEach((val, index, array) => {
          if(cls.indexOf(val.cls) == -1){ cls.push(val.cls) }
        })
        return cls
      }
      return []
    }
  }
}
</script>

<style></style>
