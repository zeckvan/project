<template>
    <div>
        <el-dialog title="檔案列表"
            :show-close="false"
            :visible.sync="dialogFormVisible"
            v-on:close="close"
            :fullscreen="false" width="100%"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <el-table :data="filelist" stripe style="width: 100%" :row-style="rowState">
                    <el-table-column type="index" width="50"></el-table-column>
                    <el-table-column prop="attestation_file_yn"
                            width="80"
                            header-align="center"
                            align="center"
                            label="確認上傳勾選"
                            v-if="this.userStatic.data_structure == 'stuattestationconfirm'"                            
                            >
                        <template slot-scope="scope">  
                            <div v-if="userStatic.checkbox">
                                <div v-if="scope.row.attestation_file_yn">
                                    <el-tag type="success" style="color: white;">已確認</el-tag>
                                </div>
                                <div v-else>
                                    <el-tag type="danger">未確認</el-tag>
                                </div>
                            </div>    
                            <div v-else>
                                <el-checkbox  :disabled="scope.row.x_centraldb > 0? true:false" v-model="scope.row.attestation_file_yn" @change="check_yn(scope)"></el-checkbox>                                  
                            </div>                                                 
                        </template>
                    </el-table-column>
                    <el-table-column prop="file_class" label="檔案類型">
                        <template slot-scope="scope">
                            <el-tag
                            :type="scope.row.file_class == 1 ? 'primary' : 'success'"
                            disable-transitions>{{scope.row.file_class == 1? '文件類':'影音類' }}
                            </el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column prop="file_name" label="檔案名稱"></el-table-column>
                    <el-table-column prop="upd_dt" label="檔案上傳時間"></el-table-column>   
                    <el-table-column prop="content" label="檔案簡述"></el-table-column>                  
                    <el-table-column label="下載檔案" width="100" align="center" v-if="userStatic.file_download">
                        <template slot-scope="scope">
                                <i class="el-icon-picture" style="cursor:pointer" @click="file_download(scope)"></i>
                        </template>
                    </el-table-column>
                    <el-table-column  label="檔案刪除" width="100" align="center" v-if="userStatic.file_delete">
                        <template slot-scope="scope">
                            <i class="el-icon-delete-solid" style="cursor:pointer"  @click="file_delete(scope)"></i>
                        </template>
                    </el-table-column>
            </el-table>
            <br/>
            <br/>
            <el-row>
                <el-col :span="22">
                    <div align="left" v-if="this.userStatic.data_structure == 'stuattestationconfirm'">
                        <div v-if="this.filelist.length > 0">
                            可確認上傳檔案數量：{{this.filelist[0].x_file_center_cnt}}  
                        </div>                                      
                    </div>                                       
                </el-col>
                <el-col :span="2">
                    <div align="right">
                        <el-button type="primary" @click="cancel">關閉視窗</el-button>
                    </div>
                </el-col>                
            </el-row>                
        </el-dialog>
    </div>
</template>

<script>
import { watch } from 'vue';
    var g_file_cnt = 0
    var g_select = 0
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
                    api_interface:{
                        type:Object
                    },
            },
            data() {
                return {
                    dialogFormVisible: this.dialog_show,
                    url: '',
                    srcList: [],
                    show_file:true,
                    show_upload:false   ,
                    //fileList: [],
                    limit:1,
                    tableData:[],
                    result:{file_cnt:0,show:false}
                }
            },
            methods: {
                check_yn:function(val){
                    var i = 0;
                    let _self = this
                    let formdata = new FormData();

                    this.filelist.forEach((element, index) => {                                               
                        if(element.attestation_file_yn == true){
                            i++
                            g_select++
                        }
                    });   
                 
                    if(i > val.row.x_file_center_cnt){
                        _self.$message.warning ("已達上傳檔案上限，請確認!!")   
                        _self.filelist[val.$index].attestation_file_yn = false
                        return false
                    }
                    
                    formdata.append('complex_key',val.row.complex_key)
                    formdata.append('number_id',val.row.number_id)
          
                    if(val.row.attestation_file_yn){
                        formdata.append('check_yn','Y')
                    }else{
                        formdata.append('check_yn','N')
                    }
                    formdata.append('token',_self.$token)

                    const loading = _self.$loading({
                        lock: true,
                        text: '資料處理中，請稍後。',
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                        });

                    const apiurl = _self.api_interface.file_checkYN
                    _self.$http({
                        url:apiurl,
                        method:"put",
                        data:formdata,
                        headers:{'SkyGet':_self.$token}
                        })
                        .then((res)=>{
                                        if (res.data.status == 'Y'){
                                            g_file_cnt = val.row.x_file_center_cnt - i
                                        }else{
                                            _self.$message.error(res.data.message)
                                        }
                                })
                        .catch((error) => {
                            _self.$message({
                            message: '異動失敗:'+error,
                            type: 'error',
                            duration:0,
                            showClose: true,
                            })
                        })
                        .finally(()=>loading.close())      
                    
                },
                rowState(row,rowindex) {
                    return {
                            backgroundColor: '#f4f4f5',
                        }
                },
                close:function() {
                    var i = 0;

                    if(this.filelist.length > 0){
                        this.filelist.forEach((element, index) => {                                               
                            if(element.attestation_file_yn == true){
                                i++
                            }
                         });     
                        this.result.file_cnt  =  i                     
                        //this.result.file_cnt  = this.filelist[0].x_file_center_cnt - i
                    }else{
                        this.result.file_cnt = 0
                    }
                    this.result.show = false                   
                    this.$emit('get-show', this.result)
                },
                cancel:function(){
                    this.dialogFormVisible = false
                },
                getshow:function(val){
                    this.isShow = val.show;
                },
                file_delete:function(val){
                    let _self = this
                    let apiurl = ''
                    const loading = _self.$loading({
                                lock: true,
                                text: '資料處理中，請稍後。',
                                spinner: 'el-icon-loading',
                                background: 'rgba(0, 0, 0, 0.7)'
                                });
                    _self.parameter.number_id = val.row.number_id

                    apiurl = _self.api_interface.file_download
                    _self.$confirm(`確定刪除?`, 'Warning', {
                        confirmButtonText: '確定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        _self.$http({
                                url:apiurl,
                                method:'delete',
                                data:_self.parameter,
                                responseType: 'blob',
                                headers:{'SkyGet':_self.$token}
                                })
                                .then((res)=>{
                                            if (res.statusText == 'OK' || res.status == '200'){
                                                _self.$message.success('檔案刪除成功!!')
                                                _self.filelist.splice(val.$index, 1);
                                            }else{
                                                _self.$message.error('檔案刪除發生錯誤!!')
                                            }
                                        })
                                .catch((error)=>{
                                        _self.$message.error('檔案刪除發生錯誤:'+error)
                                        })
                                .finally(()=>loading.close())
                    }).catch(() => {
                    }).finally(()=>loading.close())
                },
                file_download:function(val){
                    let _self = this
                    let apiurl = ''
                    const loading = _self.$loading({
                                lock: true,
                                text: '資料處理中，請稍後。',
                                spinner: 'el-icon-loading',
                                background: 'rgba(0, 0, 0, 0.7)'
                                });

                    _self.parameter.number_id = val.row.number_id
                    apiurl = _self.api_interface.file_download
                    _self.$http({
                                url:apiurl,
                                method:'get',
                                params:_self.parameter,
                                responseType: 'blob',
                                headers:{'SkyGet':_self.$token}
                                })
                                .then((res)=>{
                                        _self.download(res,val)
                                        })
                                .catch((error)=>{
                                        _self.$message.error('檔案下載發生錯誤:'+error)
                                        })
                                .finally(()=>loading.close())
                },
                download:function(res,val){
                    let _self = this
                    let context = res.data
                    let blob = new Blob([context])
                    let filename = val.row.file_name+'.'+val.row.file_extension
                    if("download" in document.createElement("a")){
                        if (res.statusText == 'OK' || res.status == '200'){
                                let link = document.createElement("a")
                                link.download = filename
                                link.style.displya = "none"
                                //link.style.displya = "block"
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
                handleRemove(file, fileList) {

                },
                handlePreview(file) {

                },
                submitUpload() {
                    this.$refs.upload.submit();
                }
            },
            mounted() {
               
            },
            beforeMount() {
                //console.log(this.filelist)
            },
            watch:{
                fileList:function(val1,val2){

                }
            }
        }
</script>

<style scoped>
.el-table__expand-icon{
        -webkit-transform: rotate(0deg);
        transform: rotate(0deg);
    }
 .el-table__expand-icon .el-icon-arrow-right:before{
        content: "\e7c3";
        font-size:18px;
        color:#303133;
        position:absolute;
        top:-4px;
        /*
        display:table-cell;
        vertical-align:middle;
        border: 1px solid #ccc;
        */
    }
.el-tag{
    color:#555555 !important
}
</style>
