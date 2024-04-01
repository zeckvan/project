import { apiHelper } from '@/utils/provider_axios.js'

const SystemSetup = {
    Get:()=>{
      return apiHelper.get('Get_L01_setup')
    },
    Update:(formdata)=>{      
      return apiHelper.put('Update_L01_setup',formdata)
    }
}

const OperateGrade = {
  Get:()=>{
    return apiHelper.get('Get_L01_operate')
  },
  Update:(formdata)=>{      
    return apiHelper.put('Update_L01_operate',formdata)
  }
}

const S90_employee = {
  Get:(parameter_tea)=>{
    return apiHelper.get(`S90_employee`,{params:parameter_tea})
  }
}

const S04_stucls_page = {
  Get:(parameter_clslist)=>{
    return apiHelper.get(`S04_stucls_page`,{params:parameter_clslist})
  }
}

const consult_setup = {
  Get:(parameter_clslist)=>{
    return apiHelper.get(`TeaConsult/Get_consult_setup`,{params:parameter_clslist})
  },
  Delete:(formdata)=>{
    return apiHelper.delete(`TeaConsult/Consult_SetUp`,{data:formdata,headers: { 'Content-Type': 'application/json;charset=utf-8' }})
  },
  Post:(formdata)=>{
    return apiHelper.post(`TeaConsult/Consult_SetUp`,formdata)
  }
}

const s90yearinfo = {
  Get:()=>{
    return apiHelper.get(`s90yearinfo`)
  }
}

const s90smsinfo = {
  Get:()=>{
    return apiHelper.get(`s90smsinfo`)
  }
}

const get_OpOpenYN = {
  Get:(parameter)=>{
    return apiHelper.get(`Get_L01_operate_open`,{params:parameter}) 
  }
}

export{
  SystemSetup,OperateGrade,S90_employee,S04_stucls_page,consult_setup,s90yearinfo,s90smsinfo,get_OpOpenYN
}