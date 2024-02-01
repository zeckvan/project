<template>
    <div>
        <el-dialog title="檔案上傳" 
            :visible.sync="dialogFormVisible" 
            v-on:close="close" 
            :fullscreen="false" width="50%"
            :show-close="false"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <div align="left">
                <el-form label-width="120px">
                    <el-row>
                        <el-upload
                                        ref="upload"
                                        action=""
                                        :on-preview="handlePreview"
                                        :on-remove="handleRemove"
                                        :on-change="handelChange"
                                        :before-upload="beforeupload"
                                        :http-request="uploadfile"
                                        list-type="picture"
                                        :limit="limit"
                                        :auto-upload="false"
                                        :accept="file_accept"
                                        >                                    
                                        <el-button slot="trigger" size="small">選擇檔案</el-button>
                                        <el-button style="margin-left: 10px;" size="small" type="info" @click="submitUpload" :disabled="check_disabled">上傳檔案</el-button>
                                        <el-divider content-position="left"><div>上傳檔案注意事項：</div></el-divider>
                                        <div slot="tip">
                                            <el-tag type="primary">文件類</el-tag>  
                                            {{docrule1}}
                                            <br/>                     
                                            <br/>                     
                                            <el-tag type="success">影音類</el-tag>                                                                    
                                            {{docrule2}}
                                        </div>                                        
                                                                                

                        </el-upload>
                    </el-row>
                    <el-divider content-position="left"><div>上傳檔案內容簡述：</div></el-divider>
                    <el-row>
                            <el-input v-model="content" type="textarea" rows = 5></el-input>
                    </el-row>
                </el-form>                            
            </div>
            <br/>                     
            <br/>                                 
            <div align="right">
                <el-button type="primary" @click="cancel">關閉視窗</el-button>
            </div>
        </el-dialog>   
    </div>
</template>
      
<script type="module">
    import * as file_structure from '@/js/fileuplod_rule.js'  

    export default {
            props: {
                    dialog_show: {
                        type: Boolean
                    },
                    filelist:{
                        type:Array
                    },
                    userStatic:{
                        type:Object
                    },       
                    parameter:{
                        type:Object
                    },    
                    data_structure:{
                        type:Object
                    },                        
                    rowdata:{
                        type:Object
                    },                                                                                                                        
                    api_interface:{
                        type:Object
                    },                         
            },
            data() {
                return {
                    dialogFormVisible: this.dialog_show,
                    show_file:true,
                    show_upload:false,
                    limit:2,
                    apiurl:"",
                    formdata:{},
                    filelisttemp:[],
                    filenametemp:"",
                    docrule1:"",
                    docrule2:"",
                    check_disabled:true,
                    file_accept:"",
                    content:"",
                    result:{show:false,filename:'',success:true},
                    success:false,
                }
            },
            methods: {
                close:function() {
                    this.result.show = false
                    this.result.success = this.success
                    this.$emit('get-show',this.result)
                },
                cancel:function(){
                    if(!this.check_disabled){
                        this.$confirm(`已挑選檔案尚未上傳是否關閉視窗?`, 'Warning', {
                        confirmButtonText: '確定',
                        cancelButtonText: '取消',
                        type: 'warning'
                        }).then(() => {
                            this.dialogFormVisible = false
                        }).catch(() => {

                        })  
                    }else{
                        this.dialogFormVisible = false
                    }                                                   
                },
                getshow:function(val){
                    this.isShow = val.show;
                },
                handelChange:function(file, fileList){
                    if(this.filenametemp == ""){
                        this.filenametemp  = file.name                        
                    }else if(this.filenametemp == file.name){
                        fileList.splice(1,1)
                    }else if(this.filenametemp != file.name){
                        fileList.splice(0,1)
                        this.filenametemp  = file.name                          
                    }                           

                    this.filelisttemp = fileList

                    var reult = file_structure.file_rule.list.filter(function(item, index, array){
                        return item.format == file.raw.type
                    });

                    if(reult.length <= 0){
                        this.$message.error('上傳格式不符合規範，請確認!!')
                        this.check_disabled = true
                        this.filenametemp = ""
                        fileList.splice(0,1)
                        return false
                    }
                    

                    if((file.raw.size/1024/1024) > reult[0].size){
                        this.$message.error('上傳大小不符合規範，請確認!!')
                        this.check_disabled = true
                        this.filenametemp = ""
                        fileList.splice(0,1)
                        return false
                    }

                    this.check_disabled = false
                    
                },
                handleRemove(file, fileList) {
                    this.filenametemp = ""
                    this.check_disabled = true
                },
                handlePreview(file) {
                },
                beforeupload(file){

                },
                uploadfile(file){

                },
                submitUpload() {
                    let _self = this
                    let apiurl  = ""
                    let formdata = new FormData();
                    let complex_key = ""

                    _self.data_structure.header.forEach(function(value, index, array){
                            if(value.parameter == "Y"){                                  
                                formdata.append(value.col,_self.rowdata.row[value.prop])
                            }                                
                        });      
                        
                    let filename = _self.filelisttemp[0].raw.name

                    _self.result.filename = filename.substr(1,filename.lastIndexOf(".") - 1)
                    formdata.append("files",_self.filelisttemp[0].raw)
                    formdata.append("class_name",_self.userStatic.interface)
                    formdata.append("content",_self.content)
 

                    if(_self.userStatic.interface == 'StuAttestation'){
                        complex_key = `${_self.rowdata.row.a}_${_self.rowdata.row.b}_${_self.rowdata.row.c}_${_self.rowdata.row.d}_${_self.rowdata.row.e}_${_self.rowdata.row.f}_${_self.rowdata.row.g}_${_self.rowdata.row.h}_0`
                        formdata.append("complex_key",complex_key)
                    }else if(_self.userStatic.interface == 'StudCadre'){
                        complex_key = `${_self.rowdata.row.a}_${_self.rowdata.row.c}_${_self.rowdata.row.d}_${_self.rowdata.row.b}_${_self.rowdata.row.e}_${_self.rowdata.row.h}`
                        formdata.append("complex_key",complex_key)                       
                    }
                    else{
                        complex_key = `${_self.rowdata.row.a}_${_self.rowdata.row.b}_${_self.rowdata.row.c}_${_self.rowdata.row.d}_${_self.rowdata.row.e}`
                        formdata.append("complex_key",complex_key)
                    }
                    formdata.append("token",_self.$token)

 /*
                    else if(_self.userStatic.interface.substr(0,3) == 'Stu'){
                        complex_key = `${_self.rowdata.row.sch_no}_${_self.rowdata.row.year_id}_${_self.rowdata.row.sms_id}_${_self.rowdata.row.std_no}_${_self.rowdata.row.ser_id}`
                        formdata.append("complex_key",complex_key)
                    }else if(_self.userStatic.interface.substr(0,3) == 'Tea'){
                        complex_key = `${_self.rowdata.row.sch_no}_
                                        ${_self.rowdata.row.year_id}_
                                        ${_self.rowdata.row.sms_id}_
                                        ${_self.rowdata.row.emp_id}_
                                        ${_self.rowdata.row.ser_id}`
                        formdata.append("complex_key",complex_key)
                    }*/                   

/*
                    switch (this.userStatic.data_structure) {
                        case 'stucadre':
                            apiurl = `${this.$apiroot}/${this.userStatic.interface}/file`
                            break;
                        case 'stustudyf':
                            apiurl = `${this.$apiroot}/absenceRecord/010106`  
                            break;                
                        default:                
                    }   
*/
                    apiurl = _self.api_interface.file_upload
                    const loading = _self.$loading({
                                    lock: true,
                                    text: '資料讀取中，請稍後。',
                                    spinner: 'el-icon-loading',
                                    background: 'rgba(0, 0, 0, 0.7)'
                                    });	

                    _self.$http({
                            //headers: { 'Content-Type': 'application/json;charset=utf-8'},
                            url:apiurl,
                            method:'post',
                            data:formdata,
                            headers:{'SkyGet':_self.$token}
                        })
                        .then((res)=>{
                                        loading.close();
                                        if (res.data.status == 'Y'){     
                                            _self.$message.success('上傳成功!!')
                                            _self.success = true
                                            _self.check_disabled = true
                                            _self.cancel()
                                        }else{
                                            _self.$message.error('上傳失敗')
                                        }     
                                })        
                        .catch((error)=>{
                                    _self.$message.error('上傳失敗，請確認:'+error)
                                })
                        .finally(()=>loading.close())                           
                },
            },
            mounted() {        
                let _self = this
                let size1 = 0
                let size2 = 0
                let doc1 = ""
                let doc2 = ""                
                file_structure.file_rule.list.forEach(function(value, index, array){
                                            if(value.type == 1){                                  
                                                size1 = value.size
                                                doc1+=value.doc+'、'
                                            }else{
                                                size2 = value.size
                                                doc2+=value.doc+'、'
                                            }               
                                            _self.file_accept+='.'+value.doc+', '
                                        });                         
               this.docrule1 = "格式限制："+doc1.substr(0,doc1.length-1)+';大小限制:'+size1+'MB'
               this.docrule2 = "格式限制："+doc2.substr(0,doc2.length-1)+';大小限制:'+size2+'MB'                 
               _self.file_accept = _self.file_accept.substr(0,_self.file_accept.length - 1)
            },     
            beforeMount(){
            },                       
        }
</script>
    
<style>
</style>
    
