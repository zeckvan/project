<template>
    <div>
        <el-form ref="form" :model="form" label-width="120px">
            <el-divider content-position="left">Basic Information</el-divider>        
            <el-row>
                <el-col :span="12">
                    <el-form-item label="學號：">
                        <el-input v-model="form.std_no" style="width:50%;margin-right:100%;" readonly></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="姓名：">
                        <el-input v-model="form.std_name" style="width:50%;margin-right:100%;" readonly></el-input>
                    </el-form-item>
                </el-col>
            </el-row>      
            <el-row>
                <el-col :span="12">
                    <el-form-item label="身份證字號：">
                        <el-input v-model="form.std_identity" style="width:50%;margin-right:100%;" readonly></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="生日：">
                        <el-input v-model="form.std_birth_dt" style="width:50%;margin-right:100%;" readonly></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-form-item label="電子郵件：">
            <el-input v-model="form.std_email" style="width:30%;margin-right:100%;" readonly></el-input>
            </el-form-item> 
            <el-divider content-position="left">Other Information</el-divider>    
            <el-row>
                <el-col :span="12">
                    <el-form-item label="FB帳號：">
                        <el-input v-model="form.std_fb" maxlength="30" show-word-limit style="width:50%;margin-right:100%;"></el-input>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="IG帳號：">
                        <el-input v-model="form.std_ig" maxlength="30" show-word-limit style="width:50%;margin-right:100%;"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-row>
                <el-col :span="12">
                    <el-form-item label="匿稱：">
                        <el-input v-model="form.std_aka" maxlength="30" show-word-limit style="width:50%;margin-right:100%;"></el-input>
                    </el-form-item>
                </el-col>
            </el-row>
            <el-form-item label="自我簡介：">
                <el-input v-model="form.std_memo" type="textarea" rows = 5></el-input>
            </el-form-item>
        </el-form>   
        <el-button type="primary" @click="save">存檔</el-button>        
        <el-button type="primary" @click="test">test</el-button>           
    </div>
</template>

<script>
    export default {
    props: {
    },        
    name: 'StuBasic',
    data() {
      return {
        form: {
          std_no: '',
          std_name: '',
          std_identity: '',
          std_birth_dt: '',
          std_email: '',
          std_fb: '',
          std_ig:'',
          std_aka:'',
          std_memo:''
        },
        file: null,
      }
    },
    methods: {
        save:function()
        {
            let _self = this
            let formdata = new FormData();

            formdata.append("std_fb",_self.form.std_fb)
            formdata.append("std_ig",_self.form.std_ig)
            formdata.append("std_nickname",_self.form.std_aka)
            formdata.append("std_memo",_self.form.std_memo)

            const loading = _self.$loading({
                    lock: true,
                    text: '資料讀取中，請稍後。',
                    spinner: 'el-icon-loading',
                    background: 'rgba(0, 0, 0, 0.7)'
                    });

            const apiurl = `${this.$apiroot}/studentinfo`    
            _self.$http({
                url:apiurl,
                method:'post',
                data:formdata,
                headers:{'SkyGet':_self.$token}
                })
                .then((res)=>{
                                if (res.data.status == 'Y'){
                                    _self.$message.success('存檔成功!!')
                                }else{
                                    _self.$message.error(res.data.message)
                                }
                        })
                .catch((error) => {
                    _self.$message({
                    message: '存檔失敗:'+error,
                    type: 'error',
                    duration:0,
                    showClose: true,
                    })
                })
                .finally(()=>loading.close())
        },
        test:function(){
            let _self = this
                    let apiurl = ''
                    const loading = _self.$loading({
                                lock: true,
                                text: '資料處理中，請稍後。',
                                spinner: 'el-icon-loading',
                                background: 'rgba(0, 0, 0, 0.7)'
                                });    
                    //apiurl = 'http://localhost/zeck/api/filedownload/file/190406;84e809df-be44-4b72-8ae0-b155c6b8fed1'
                    apiurl = 'http://localhost/learnfile/api/filedownload/file/190406;84e809df-be44-4b72-8ae0-b155c6b8fed1'
                    //apiurl = `${this.$apiroot}/stufileinfo/filedownload/190406;84e809df-be44-4b72-8ae0-b155c6b8fed1`  
                    _self.$http({
                                url:apiurl,
                                method:'get',
                                responseType: 'blob',
                                headers:{'SkyGet':_self.$token}
                                })
                                .then((res)=>{
                                            var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/
                                            var matches = filenameRegex.exec(decodeURI(res.headers['content-disposition'].split(';')[2]))
                                            if (matches != null && matches[1]) {                                                 
                                                var filename = matches[1].replace(/['"]/g, '').replace('UTF-8','')
                                                _self.download(res,filename)
                                            }
                                        })
                                .catch((error)=>{
                                        _self.$message.error('檔案下載發生錯誤:'+error)
                                        })
                                .finally(()=>loading.close())            
        },
                download:function(res,filename){
                    let _self = this
                    let context = res.data
                    let blob = new Blob([context])
                    if("download" in document.createElement("a")){
                        if (res.statusText == 'OK' || res.status == '200'){
                                let link = document.createElement("a")
                                link.download = filename
                                link.style.displya = "none"
                                link.href = URL.createObjectURL(blob)
                                document.body.appendChild(link)
                                link.click()
                                URL.revokeObjectURL(link.href)
                                document.body.removeChild(link)
                        }else{
                            _self.$message.error('檔案下載發生錯誤!!')
                        }
                    }else{
                        navigator.msSaveBold(blob,filename)
                    }
                },
        formatDate: function (date) {
            var d = new Date(date),
                            month = '' + (d.getMonth() + 1),
                            day = '' + d.getDate(),
                            year = d.getFullYear();
                            
                        if (month.length < 2) 
                            month = '0' + month;
                        if (day.length < 2) 
                            day = '0' + day;
                return [year, month, day].join('-')
            }
    },
    mounted: function () {
        let _self = this       
        const apiurl = `${this.$apiroot}/studentinfo/${_self.$token}`  
        console.log(apiurl)    
        _self.$http({
                url:apiurl,
                method:'get',
                //crossdomain:true,
                headers: {"SkyGet":_self.$token },
                })
                .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.form.std_no = res.data.dataset.std_no
                                _self.form.std_name = res.data.dataset.std_name
                                _self.form.std_birth_dt = this.formatDate(res.data.dataset.std_birth_dt)
                                _self.form.std_identity = res.data.dataset.std_identity
                                _self.form.std_email = res.data.dataset.std_email
                                _self.form.std_fb = res.data.dataset.std_fb
                                _self.form.std_ig = res.data.dataset.std_ig
                                _self.form.std_aka = res.data.dataset.std_nickname
                                _self.form.std_memo = res.data.dataset.std_memo
                            }else{
                                _self.$message.error('查無資料，請確認')
                            }
                        })
                .catch((error)=>{
                                console.log(error)
                    })
                .finally()
    }
  }
</script>

<style></style>
