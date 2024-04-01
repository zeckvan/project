import { apiHelper } from '@/utils/provider_axios.js'

const StudCadre = {
    GetList:(parameter)=>{        
        return apiHelper.get('StudCadre/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StudCadre',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StudCadre',formdata)
    },
    Delete:(formdata)=>{
        return apiHelper.delete('StudCadre',{data:formdata,headers: { 'Content-Type': 'application/json;charset=utf-8' }})
    },
    Post:(formdata)=>{
        return apiHelper.post('StudCadre',formdata)
    },
    save_centraldb:()=>{
        return apiHelper.put('StudCadre/centraldb',formdata)
    }
}

const StuCompetition = {
    GetList:(parameter)=>{
        return apiHelper.get('StuCompetition/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuCompetition',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuCompetition',formdata)
    },
    save_centraldb:(formdata)=>{
        return apiHelper.put('StuCompetition/centraldb',formdata)
    }
}

const StuLicense = {
    GetList:(parameter)=>{
        return apiHelper.get('StuLicense/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuLicense',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuLicense',formdata)
    }
}

const StuVolunteer = {
    GetList:(parameter)=>{
        return apiHelper.get('StuVolunteer/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuVolunteer',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuVolunteer',formdata)
    }
}

const StuResult = {
    GetList:(parameter)=>{
        return apiHelper.get('StuResult/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuResult',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuResult',formdata)
    }
}

const StuOther = {
    GetList:(parameter)=>{
        return apiHelper.get('StuOther/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuOther',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuOther',formdata)
    }
}

const StuStudyf = {
    GetList:(parameter)=>{
        return apiHelper.get('StuStudyf/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuStudyf',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuStudyf',formdata)
    }
}

const StuCollege = {
    GetList:(parameter)=>{
        return apiHelper.get('StuCollege/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuCollege',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuCollege',formdata)
    }
}

const StuWorkPlace = {
    GetList:(parameter)=>{
        return apiHelper.get('StuWorkPlace/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuWorkPlace',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuWorkPlace',formdata)
    }
}

const StuGroup = {
    GetList:(parameter)=>{
        return apiHelper.get('StuGroup/list',{params:parameter})
    },
    Get:(parameter)=>{
        return apiHelper.get('StuGroup',{params:parameter})
    },
    Put:(formdata)=>{
        return apiHelper.put('StuGroup',formdata)
    }
}

const studiversecheck = {
    Get:(parameter)=>{
        return apiHelper.get('Get_Diverse_Total',{params:parameter})
    }
}

const StuConsult = {
    GetList:(parameter)=>{
        return apiHelper.get('StuConsult/list',{params:parameter})
    }
}

const stu_attestation = {
    GetList:(parameter)=>{
        return apiHelper.get('StuAttestation/courselist',{params:parameter})
    },
    Delete:(formdata)=>{
        return apiHelper.delete('StuAttestation',{data:{argdata:formdata},headers: { 'Content-Type': 'application/json;charset=utf-8' }})
    }
}

export{
    StudCadre,
    StuCompetition,
    StuLicense,
    StuVolunteer,
    StuResult,
    StuOther,
    StuStudyf,StuCollege,StuWorkPlace,StuGroup,studiversecheck,StuConsult,stu_attestation}
