<template>
  <div>
    <el-form :model="form" :rules="rules" ref="form" label-width="100px">
      <!--
      <el-row>
        <el-col :span="12">
          <el-form-item label="學年" prop="year">
            <el-select v-model="form.year">
              <el-option v-for="y in year_option" :label="y.year_name" :value="y.year_id"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="學期" prop="sms">
            <el-select v-model="form.sms">
              <el-option v-for="s in sms_option" :label="s.sms_name" :value="s.sms_id"></el-option>
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      -->

      <el-row>
        <el-col :span="12">
          <el-form-item label="解壓縮密碼" prop="password">
            <el-input v-model="form.password"></el-input>
          </el-form-item>

          <el-form-item label="MD5" prop="md5">
            <el-input v-model="form.md5"></el-input>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="壓縮檔" prop="file">
            <input ref="file" class="el-input" type="file" @change="onFileChange">
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col>
          <el-form-item>
            <el-button type="primary" native-type="button" @click="submitForm('form')">上傳</el-button>
            <el-button @click="resetForm('form')">重置</el-button>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <el-alert v-if="is_success == 'Y'"
      title="匯入成功!!"
      type="success"
      description=""
      show-icon>
    </el-alert>    
    <el-alert v-else-if="is_success == 'N'"
      title="匯入失敗"
      type="error"
      :description=error
      show-icon>
    </el-alert>    
  </div>
</template>

<script type="module">
import PubQuery from '@/components/pub/pub_query2.vue'

export default {
  props: {
    userStatic: {
      type: Object,
    },
    api_interface: {
      type: Object,
    },
    total:{
      type:Number
    }
  },
  data() {
    return {
      year_option: [],
      sms_option: [],
      form: {
        year: "",
        sms: "",
        password: "",
        md5:"",
        file: null,
      },
      error:"",
      is_success:'',
      rules: {
        year: [
          { required: true, message: '請選擇學年', trigger: 'change' },
        ],
        sms: [
          { required: true, message: '請選擇學期', trigger: 'change' }
        ],
        password: [
          { required: true, message: '請輸入密碼', trigger: 'change' }
        ],
        file: [
          { required: true, message: '請選擇', trigger: 'change' }
        ],
      }
    }
  },
  computed: {
    selectYear() {
      let option = new Date().getFullYear() - 1911;
      option += 1;
      let options = [option];
      for (let i = 0; i < 6; i++) {
        options.push(--option);
      }
      return options;
    }
  },
  methods: {
    submitForm(formName) {
      let _self = this

      this.$refs[formName].validate((valid) => {
        if (valid) {
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          const apiurl = _self.api_interface.import_file

          let form = new FormData()
          //form.append("year", _self.form.year)
          //form.append("sms", _self.form.sms)
          form.append("Password", _self.form.password)
          form.append("files", _self.form.file[0])
          //form.append("token", _self.$token)

          _self.$http({
            url: apiurl,
            method: 'post',
            data: form,
            headers: { "Content-Type": "multipart/form-data",'SkyGet':_self.$token },
          })
            .then((res) => {
              if (res.data.status == 'Y') {
                _self.is_success = 'Y'
                //_self.$message.success('匯入成功!!')

                // _self.form.year = ""
                // _self.form.sms = ""
                // _self.form.password = ""
                // _self.form.file = null
              } else {
                _self.$message.error(res.data.error_msg)
              }
            })
            .catch((error) => {
              _self.error = error
              _self.is_success = 'N'
              // _self.$message({
              //   message: '匯入失敗:' + error,
              //   type: 'error',
              //   duration: 0,
              //   showClose: true,
              // })
            })
            .finally(
              () => {
                _self.resetForm('form')
                loading.close()
              })
        }
      });
    },
    resetForm(formName) {
      this.$refs["file"].value = "";
      this.$refs[formName].resetFields();
    },
    onFileChange(e) {
      let files = e.target.files || e.dataTransfer.files;
      if (!files.length) { return; }
      this.form.file = files;
    },
    getsmsOption() {
      let _self = this

      const apiurl = `${_self.$apiroot}/s90smsinfo`
      _self.$http({
        url: apiurl,
        method: 'get',
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {
          _self.sms_option = res.data.dataset.filter(function (value) {
            return value.sms_id == 1 || value.sms_id == 2;
          });
        })
        .catch((error) => {
          _self.$message.error('呼叫後端【s90smsinfo】發生錯誤,' + error)
        })
        .finally()
    },
    getyearOption() {
      let _self = this
      const apiurl = `${_self.$apiroot}/s90yearinfo`
      _self.$http({
        url: apiurl,
        method: 'get',
        headers:{'SkyGet':_self.$token}
      })
      .then((res) => {
        _self.year_option = res.data.dataset
      })
      .catch((error) => {
        _self.$message.error('呼叫後端【s90yearinfo】發生錯誤,' + error)
      })
      .finally()
    }
  },
  mounted: function () {
    this.getyearOption();
    this.getsmsOption();
  },
}
</script>

<style></style>
