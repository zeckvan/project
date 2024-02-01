<template>
    <div>
        <el-dialog title="社團、校外、其它幹部經歷資料建立" 
            :visible.sync="dialogFormVisible" 
            v-on:close="close" 
            :fullscreen="false" width="100%"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <el-form ref="ruleForm" :model="form" :rules="rules" label-width="120px">
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="學年度：" prop="year_id">
                            <datalistyear v-on:get-year="getyear"  :year_id="form.year_id" placeholder="" style="width:50%;margin-right:100%;" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                </el-col>                    
                    <el-col :span="8">
                        <el-form-item label="學期：" prop="sms_id" placeholder="" style="width:50%;margin-right:100%;">
                            <datalistsms v-on:get-sms="getsms"   :sms_type="sms_type" :sms_id="form.sms_id" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="單位名稱：" prop="unit_name">
                            <el-input v-model="form.unit_name" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="擔任職務：" prop="position_name">
                            <el-input v-model="form.position_name" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="幹部等級：" prop="type_id">
                            <el-select v-model="form.type_id" placeholder="" style="width:75%;margin-right:100%;" :disabled="is_readonly? true:false">
                                <el-option v-for="item in typeid" :key="item.id" :label="item.name" :value="item.id" >
                                </el-option>        
                            </el-select>                                 
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="開始日期：" prop="startdate">
                            <el-date-picker :clearable="true" v-model="form.startdate" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                v-on:change="begdate_chk_range"
                                v-on:focus="begdate_chk_value"
                                :readonly=" is_readonly">
                            </el-date-picker>                                                     
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="結束日期：" prop="enddate">
                            <el-date-picker :clearable="true" v-model="form.enddate" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                v-on:change="enddate_chk_range"
                                v-on:focus="enddate_chk_value"
                                :readonly=" is_readonly">
                            </el-date-picker>                                            
                        </el-form-item>
                    </el-col>
                </el-row>
                <!--
                <el-form-item label="內容簡述：" prop="std_memo">
                    <el-input v-model="form.std_memo" type="textarea" rows = 5></el-input>
                </el-form-item> 
                -->
            </el-form> 
            <div v-if="SaveButtonVisible==false">
                <el-button type="primary" @click="save" :disabled="SaveButtonVisible">確定存檔</el-button>
                <el-button type="primary" @click="cancel">關閉視窗</el-button>
            </div>
            <div v-else>
                <el-button type="primary" @click="cancel">關閉視窗</el-button>
            </div>
        </el-dialog>    
    </div>
</template>
  
<script type="module">
    import datalistyear from '@/components/pub/DataList_year.vue'  
    import datalistsms from '@/components/pub/DataList_sms.vue'    
  
    export default {
            props: {
                dialog_show: {
                    type: Boolean
                },
                edit_model:{
                    tyep:String
                },
                data_structure:{
                    type:Object
                },
                parameter:{
                    type:Object
                },
                userStatic: {
                    type: Object,
                },
                api_interface:{
                    type: Object,
                },
                formSaveCheck:{
                    type:String
                }
            },
            data() {
                return {
                            sms_type: '1',
                            form: {year_id:'',sms_id:'',unit_name:'',position_name:'',type_id:'',startdate:'',enddate:'',is_sys:''},                
                            rules: {
                                year_id: [
                                        { required: true, message: '請輸入學年', trigger: 'change' }
                                    ],
                                sms_id: [
                                    { required: true, message: '請輸入學期', trigger: 'change' }
                                ],
                                unit_name: [
                                    { required: true, message: '請輸入單位名稱', trigger: 'change' }
                                ],                                                                        
                                position_name: [
                                    { required: true, message: '請輸入擔任職務', trigger: 'change' }
                                ],                                
                                type_id: [
                                    { required: true, message: '請輸入幹部等級', trigger: 'change' }
                                ],                                                                
                            },
                            dialogFormVisible: this.dialog_show,
                            typeid:[{id:"1",name:"校級幹部"},
                                    {id:"2",name:"班級幹部"},
                                    {id:"3",name:"社團幹部"},
                                    {id:"4",name:"實習幹部"},
                                    {id:"5",name:"校外自治組織團體"},
                                    {id:"9",name:"其他幹部"}],
                            is_readonly :false,
                            method_type:"post",
                            SaveButtonVisible:false      
                         }
            },
            methods: {
                    cancel:function(){
                        this.dialogFormVisible = false
                    },
                    save:function(){
                        let _self = this
                        let formdata = new FormData();

                        this.data_structure.header.forEach(function(value, index, array){
                                formdata.append(value.col,_self.form[value.col]);
                            });
                        formdata.append("token",_self.$token)

                        _self.$refs["ruleForm"].validate((valid) => {
                            if (valid) {
                                const loading = _self.$loading({
                                        lock: true,
                                        text: '資料讀取中，請稍後。',
                                        spinner: 'el-icon-loading',
                                        background: 'rgba(0, 0, 0, 0.7)'
                                        });	
                                    
                                const apiurl = _self.api_interface.save_form
                                _self.$http({
                                    //'Content-Type': 'multipart/form-data',
                                    url:apiurl,
                                    method:_self.method_type,
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
                                    .catch((error)=>{
                                                _self.$message.error('存檔失敗:'+error)
                                            })
                                    .finally(()=>loading.close())                          
                                this.dialogFormVisible = false
                            }
                            else{
                                return false     
                            }
                        })                                               
                    },
                    close:function() {
                        this.$emit('get-show', false)
                    },
                    getyear:function(val){
                        this.form.year_id = val
                    },                
                    getsms:function(val){
                         this.form.sms_id = val
                    },      
                    enddate_chk_range:function(){
                        if (this.form.startdate > this.form.enddate) {
                            this.form.enddate = ''
                            this.$message.error('結束日期不可小於開始日期，請確認。');
                        }                        
                        if (this.form.startdate === "" || this.form.startdate === null) {
                            this.form.enddate = ''
                        }                                                
                    },      
                    enddate_chk_value:function(){
                        if (this.form.startdate === "" || this.form.startdate === null) {
                            this.$message.error('請先輸入開始日期，請確認。');
                            this.form.enddate = ''
                        }                        
                    },      
                    begdate_chk_range:function(){
                        if (this.form.enddate != "" && this.form.enddate != null) {
                            if (this.form.startdate > this.form.enddate) {
                                 this.form.startdate = ''
                                 this.form.enddate = ''
                                 this.$message.error('開始日期不可大於結束日期，請確認。');
                            }                        
                        }   
                        
                        if(this.form.startdate === null){
                            this.form.startdate = ''
                            this.form.enddate = ''
                            this.$message.error('開始日期不可大於結束日期，請確認。');
                        }
                    },      
                    begdate_chk_value:function(){
                    }
            },
            mounted() {
                let _self = this
                if(this.formSaveCheck == 'studiversecheck'){
                    this.SaveButtonVisible = true
                }else{
                    this.SaveButtonVisible = false
                }
                if(this.edit_model === "add"){                                          
                    this.is_readonly = false
                    this.method_type = "post"
                    this.data_structure.header.forEach(function(value, index, array){
                                                _self.form[value.col] = ''
                                          });
                    this.form.is_sys = "2"                                          
                }else{         
                    const loading = _self.$loading({
                                    lock: true,
                                    text: '資料讀取中，請稍後。',
                                    spinner: 'el-icon-loading',
                                    background: 'rgba(0, 0, 0, 0.7)'
                                    });	
                                
                    const apiurl = _self.api_interface.get_form
                    _self.$http({
                        url:apiurl,
                        method:'get',
                        params:this.parameter,
                        headers:{'SkyGet':_self.$token}
                        })
                        .then((res)=>{
                                        if (res.data.status == 'Y'){            
                                            this.data_structure.header.forEach(function(value, index, array){
                                                    if(res.data.dataset[value.col]){
                                                        if(value.col == "year_id" || value.col == "sms_id"){
                                                            _self.form[value.col] = res.data.dataset[value.col].toString()
                                                        }else if(value.col == "startdate" || value.col == "enddate"){
                                                            _self.form[value.col] = res.data.dataset[value.col] == "undefined" ? "":res.data.dataset[value.col]
                                                        }else{
                                                            _self.form[value.col] = res.data.dataset[value.col]
                                                        }                                                    
                                                    }
                                            })
                                            _self.is_readonly = (res.data.dataset.is_sys === "1"? true : false)
                                        }else{
                                            _self.$message.error('查無資料，請確認')
                                        }     
                                })        
                        .catch((error)=>{
                                            _self.$message.error('系統發生錯誤:'+error)
                                })
                        .finally(()=>loading.close())                          
                        this.method_type = "put"
                    }
            },      
            components: {
                datalistyear,
                datalistsms    
            },  
    }
</script>
  
<style>

</style>
  