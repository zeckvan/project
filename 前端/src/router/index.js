import Vue from "vue"
import VueRouter from "vue-router"
import StuBasicView from "../views/StuBasicView.vue"
import IndexView from "../views/IndexView.vue"
import * as stuAPI from  '@/apis/stuApi.js'


//Vue.prototype.$apiroot = window.apiConfig.api
/**
let cookieObj = {};
let cookieAry = document.cookie.split(';');
let cookie;
parseCookie()
function parseCookie() {
  for (let i=0, l=cookieAry.length; i<l; ++i) {//
      if(cookieAry[i].includes('=')){
        cookie = cookieAry[i].split('=');
        cookieObj[cookie[0]] = cookie[1];
      }
  }
}

Vue.prototype.$token = cookieObj[' X-Token']
Vue.prototype.$token = '97814c3b'
 */

Vue.use(VueRouter)

const routes = [
  {
    path: "/",
    name: "root",
    component: () => import("../views/IndexView.vue"),
  },
  {
    path: "/stubasic",
    name: "stubasic",
    component: () => import("../components/student/stu_basic.vue"),
    //component: () => import("../views/StuBasicView.vue"),
  },
  {
    path: "/StuAddendDetail",
    name: "StuAddendDetail",
    component: () => import("../components/student/stu_addend_detail.vue"),
  },
  {
    path: "/StuRewardDetail",
    name: "StuRewardDetail",
    component: () => import("../components/student/stu_reward_detail.vue"),
  },
  {
    path: "/stucadre",//幹部經歷
    name: "stucadre",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stucadre",
        form_component: "v-stucadreform",
        interface: "StudCadre",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        get_form: `${Vue.prototype.$apiroot}/StudCadre`,
        save_form: `${Vue.prototype.$apiroot}/StudCadre`,
        //save_centraldb:`${Vue.prototype.$apiroot}/StudCadre/centraldb`,
        save_centraldb:(formdata)=>{
          return stuAPI.StudCadre.save_centraldb(formdata)
        },
        get_data:(parameter)=>{
          return stuAPI.StudCadre.GetList(parameter)
        },
        del_data:(formdata)=>{
          return stuAPI.StudCadre.Delete(formdata)
        }
      }
    }
  },
  {
    path: "/stustudyf",//彈性學習時間
    name: "stustudyf",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stustudyf",
        form_component: "v-stustudyform",
        interface: "StuStudyf",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        /*
        file_download: `${Vue.prototype.$apiroot}/StuStudyf/file`,
        file_data: `${Vue.prototype.$apiroot}/StuStudyf/files`,
        file_upload: `${Vue.prototype.$apiroot}/StuStudyf/file`,
        */
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data: `${Vue.prototype.$apiroot}/StuStudyf`,
        //get_data: `${Vue.prototype.$apiroot}/StuStudyf/list`,
        get_form: `${Vue.prototype.$apiroot}/StuStudyf`,
        save_form: `${Vue.prototype.$apiroot}/StuStudyf`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuStudyf/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuStudyf.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stugroup",//團體活動時間記錄
    name: "stugroup",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stugroup",
        form_component: "v-stugroupform",
        interface: "StuGroup",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuGroup`,
        //get_data:`${Vue.prototype.$apiroot}/StuGroup/list`,
        get_form:`${Vue.prototype.$apiroot}/StuGroup`,
        save_form:`${Vue.prototype.$apiroot}/StuGroup`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuGroup/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuGroup.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stucollege",//大專先修課程
    name: "stucollege",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stucollege",
        form_component: "v-stucollegeform",
        interface: "StuCollege",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuCollege`,
        //get_data:`${Vue.prototype.$apiroot}/StuCollege/list`,
        get_form:`${Vue.prototype.$apiroot}/StuCollege`,
        save_form:`${Vue.prototype.$apiroot}/StuCollege`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuCollege/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuCollege.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stuworkplace",//職場學習
    name: "stuworkplace",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuworkplace",
        form_component: "v-stuworkplaceform",
        interface: "StuWorkPlace",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuWorkPlace`,
        //get_data:`${Vue.prototype.$apiroot}/StuWorkPlace/list`,
        get_form:`${Vue.prototype.$apiroot}/StuWorkPlace`,
        save_form:`${Vue.prototype.$apiroot}/StuWorkPlace`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuWorkPlace/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuWorkPlace.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stuother",//其它活動紀錄
    name: "stuother",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuother",
        form_component: "v-stuotherform",
        interface: "StuOther",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuOther`,
        //get_data:`${Vue.prototype.$apiroot}/StuOther/list`,
        get_form:`${Vue.prototype.$apiroot}/StuOther`,
        save_form:`${Vue.prototype.$apiroot}/StuOther`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuOther/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuOther.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/sturesult",//作品成果紀錄
    name: "sturesult",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "sturesult",
        form_component: "v-sturesultform",
        interface: "StuResult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuResult`,
        //get_data:`${Vue.prototype.$apiroot}/StuResult/list`,
        get_form:`${Vue.prototype.$apiroot}/StuResult`,
        save_form:`${Vue.prototype.$apiroot}/StuResult`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuResult/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuResult.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stuvolunteer",//志工服務紀錄
    name: "stuvolunteer",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuvolunteer",
        form_component: "v-stuvolunteerform",
        interface: "StuVolunteer",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuVolunteer`,
        //get_data:`${Vue.prototype.$apiroot}/StuVolunteer/list`,
        get_form:`${Vue.prototype.$apiroot}/StuVolunteer`,
        save_form:`${Vue.prototype.$apiroot}/StuVolunteer`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuVolunteer/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuVolunteer.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stulicense",//檢定證照紀錄
    name: "stulicense",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stulicense",
        form_component: "v-stulicenseform",
        interface: "StuLicense",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuLicense`,
        //get_data:`${Vue.prototype.$apiroot}/StuLicense/list`,
        get_form:`${Vue.prototype.$apiroot}/StuLicense`,
        save_form:`${Vue.prototype.$apiroot}/StuLicense`,
        save_centraldb:`${Vue.prototype.$apiroot}/StuLicense/centraldb`,
        get_data:(parameter)=>{
          return stuAPI.StuLicense.GetList(parameter)
        },
      }
    }
  },
  {
    path: "/stucompetition",//競賽參與紀錄
    name: "stucompetition",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stucompetition",
        form_component: "v-stucompetitionform",
        interface: "StuCompetition",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuCompetition`,
        get_form:`${Vue.prototype.$apiroot}/StuCompetition`,
        save_form:`${Vue.prototype.$apiroot}/StuCompetition`,
        get_data:(parameter)=>{
          return stuAPI.StuCompetition.GetList(parameter)
        },
        save_centraldb:(formdata)=>{
          return stuAPI.StuCompetition.save_centraldb(formdata)
        },
      }
    }
  },
  {
    path: "/teaconsult",//學生諮詢課程填寫
    name: "teaconsult",
    component: () => import("../views/TeaConsultView.vue"),
    props: {
      userStatic: {
        data_structure: "teaconsult",
        form_component: "v-teaconsultform",
        interface: "TeaConsult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/TeaConsult`,
        get_data:`${Vue.prototype.$apiroot}/TeaConsult/list`,
        get_form:`${Vue.prototype.$apiroot}/TeaConsult`,
        save_form:`${Vue.prototype.$apiroot}/TeaConsult`,
        get_stuconsult:`${Vue.prototype.$apiroot}/TeaConsult/consultstulist`,
        get_studata:`${Vue.prototype.$apiroot}/TeaConsult/stulist`,
        del_studata:`${Vue.prototype.$apiroot}/TeaConsult/stulist`,
        del_stuconsult:`${Vue.prototype.$apiroot}/TeaConsult/consultstulist`,
        save_studata:`${Vue.prototype.$apiroot}/TeaConsult/stulist`,
        save_stuconsult:`${Vue.prototype.$apiroot}/TeaConsult/consultstulist`,
      }
    }
  },
  {
    path: "/consultSetUp",//學生諮詢課程教師設定
    name: "consultSetUp",
    component: () => import("../components/admin/consult_cls_setup.vue"),
    props: {
      userStatic: {
        data_structure: "consult_cls",
        form_component: "",
        interface: "ConsultSetUp",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        insertConsult_SetUp:`${Vue.prototype.$apiroot}/TeaConsult/Consult_SetUp`,
        deleteConsult_SetUp:`${Vue.prototype.$apiroot}/TeaConsult/Consult_SetUp`,
        get_tealist:`${Vue.prototype.$apiroot}/Public`,
        get_clslist:`${Vue.prototype.$apiroot}/Public`,
        get_consult:`${Vue.prototype.$apiroot}/Public`,
      }
    }
  },
  {
    path: "/stuconsultQuery",//學生諮詢課程查詢
    name: "stuconsultQuery",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuconsult_query",
        form_component: "v-stuconsultQueryform",
        interface: "TeaConsult",
        file_delete:false,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        //get_data:`${Vue.prototype.$apiroot}/StuConsult/list`,
        get_form:`${Vue.prototype.$apiroot}/StuConsult`,
        get_data:(parameter)=>{
          return stuAPI.StuConsult.GetList(parameter)
        }
      }
    }
  },
  {
    path: "/stuattestation",//上傳課程學習成果
    name: "stuattestation",
    component: () => import("../components/student/stu_attestation.vue"),
    props: {
      userStatic: {
        data_structure: "stuattestation",
        form_component: "v-stuattestationform",
        interface: "StuAttestation",
        file_delete:false,
        file_download:true,
        file_upload:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuAttestation`,
        get_data:`${Vue.prototype.$apiroot}/StuAttestation/courselist`,
        //get_data:`${Vue.prototype.$apiroot}/StuAttestation/list`,
        get_form:`${Vue.prototype.$apiroot}/StuAttestation`,
        save_form:`${Vue.prototype.$apiroot}/StuAttestation`,
        get_OpOpenYN:`${Vue.prototype.$apiroot}/Get_L01_operate_open`,
        get_data:(parameter)=>{
          return stuAPI.stu_attestation.GetList(parameter)
        }
      }
    }
  },
  {
    path: "/stuattestationconfirm",//確認課程學習成果
    name: "stuattestationconfirm",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuattestationconfirm",
        form_component: "",
        interface: "StuAttestation",
        file_delete:false,
        file_download:true,
        checkbox:false
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_checkYN:`${Vue.prototype.$apiroot}/StuAttestation/fileCheckYN`,
        del_data:`${Vue.prototype.$apiroot}/StuAttestation`,
        get_data:`${Vue.prototype.$apiroot}/StuAttestation/listconfirm`,
        get_form:`${Vue.prototype.$apiroot}/StuAttestation`,
        save_form:`${Vue.prototype.$apiroot}/StuAttestation/confirm`,
      }
    }
  },
  {
    path: "/stuattestationcentraldb",//確認課程學習成果
    name: "stuattestationcentraldb",
    component: () => import("../components/student/stu_pub_grid.vue"),
    props: {
      userStatic: {
        data_structure: "stuattestationcentraldb",
        form_component: "",
        interface: "StuAttestation",
        file_delete:true,
        file_download:true
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuAttestation`,
        get_data:`${Vue.prototype.$apiroot}/StuAttestation/listcentraldb`,
        get_form:`${Vue.prototype.$apiroot}/StuAttestation`,
        save_form:`${Vue.prototype.$apiroot}/StuAttestation/centraldb`,
        get_OpOpenYN:`${Vue.prototype.$apiroot}/Get_L01_operate_open`,
      }
    }
  },
  {
    path: "/centerDB-stdfeedback",//中央資料庫（學生）-問題回報
    name: "centerDB-stdfeedback",
    component: () => import("../components/centerDB/std/centerDB_std_feedback.vue"),
    props: {
      std_urlObject: {
        GetYmsStd: '/CentralDBofLearningHistory/GetYmsStd',
        FeedBackKindCls:'/CentralDBofLearningHistory/FeedBackKindCls',
        FeedBackOpenKind:'/CentralDBofLearningHistory/FeedBackOpenKind',
        StdFeedback:'/CentralDBofLearningHistory/StdFeedBack',
        FeedBackErrorCode:'/CentralDBofLearningHistory/FeedBackErrorCode',
      },
    }
  },
  {
    path: "/centerDB-stdquery",//中央資料庫（學生）-查詢提交記錄
    name: "centerDB-stdquery",
    component: () => import("../components/centerDB/centerDB_query_std.vue"),
    props: {
      std_urlObject: {
        QueryStd: '/CentralDBofLearningHistory/GetClsStd',
        StdCheckData: '/CentralDBofLearningHistory/PutStdCheckData',
        GetStdYms: '/CentralDBofLearningHistory/GetYmsStd',
      },
    }
  },
  {
    path: "/centerDB",//中央資料庫-收訖（承辦人員）
    name: "centerDB",
    component: () => import("../views/CenterDBView.vue"),
  },
  {
    path: "/ExportExcel",//匯出提交名冊（承辦人員）
    name: "ExportExcel",
    component: () => import("@/components/admin/export_excel.vue"),
  },
  {
    path: "/teaattestation",//授課教師確認課程學習成果
    name: "teaattestation",
    component: () => import("../views/TeaAttestationView.vue"),
    props: {
      userStatic: {
        data_structure: "teaattestation",
        form_component: "",
        interface: "StuAttestation",
        file_delete:false,
        file_download:true
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/TeaAttestation`,
        get_Verify:`${Vue.prototype.$apiroot}/TeaAttestation/listconfirm`,
        get_Release:`${Vue.prototype.$apiroot}/TeaAttestation/listrelease`,
        get_form:`${Vue.prototype.$apiroot}/TeaAttestation`,
        save_Verify:`${Vue.prototype.$apiroot}/TeaAttestation/confirm`,
        save_Release:`${Vue.prototype.$apiroot}/TeaAttestation/confirmRelease`,
        save_Reason:`${Vue.prototype.$apiroot}/TeaAttestation/confirmReason`,
        get_OpOpenYN:`${Vue.prototype.$apiroot}/Get_L01_operate_open`,
      }
    }
  },
  {
    path: "/teaattestationquery",//授課教師查看課程學習成果
    name: "teaattestationquery",
    component: () => import("../components/teacher/tea_attestation_query.vue"),
    props: {
      userStatic: {
        data_structure: "tea_attestation_query",
        form_component: "",
        interface: "",
        file_delete:false,
        file_download:true
      },
      api_interface:
      {
        get_attestationResult:`${Vue.prototype.$apiroot}/TeaAttestation/attestationResult`,
      }
    }
  },
  {
    path: "/studiversecheck",//勾選多元表現
    name: "studiversecheck",
    component: () => import("../components/student/stu_diverse_check.vue"),
    props: {
      userStatic: {
        data_structure: "studiversecheck",
        form_component: "",
        interface: "",
        file_delete:false,
        file_download:true,
      },
      api_interface:
      {
        file_download:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        file_data:`${Vue.prototype.$apiroot}/StuFileInfo/files`,
        file_upload:`${Vue.prototype.$apiroot}/StuFileInfo/file`,
        del_data:`${Vue.prototype.$apiroot}/StuCompetition`,
        get_data:`${Vue.prototype.$apiroot}/StuCompetition/list`,
        get_form:`${Vue.prototype.$apiroot}/StuCompetition`,
        save_form:`${Vue.prototype.$apiroot}/StuCompetition`,
        get_OpOpenYN:`${Vue.prototype.$apiroot}/Get_L01_operate_open`,
        get_Diverse_Total:`${Vue.prototype.$apiroot}/Get_Diverse_Total`,
        //get_data:(parameter)=>{
          //return stuAPI.StuOther.GetList(parameter)
        //},
      }
    }
  },
  {
    path: "/teatutor",//導師查看學生學習成果及多元表現
    name: "teatutor",
    component: () => import("../views/TeaTutorView.vue"),
    props: {
      userStatic: {
        data_structure: "teatutor",
        form_component: "",
        interface: "teatutor",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        get_data:`${Vue.prototype.$apiroot}/TeaAttestation/teaTutor`,
      }
    }
  },
  {
    path: "/teaconsultquery",//課程諮詢教師查看學生學習成果及多元表現
    name: "teaconsultquery",
    component: () => import("../views/TeaTutorView.vue"),
    props: {
      userStatic: {
        data_structure: "teatutor",
        form_component: "",
        interface: "teaconsult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        get_data:`${Vue.prototype.$apiroot}/TeaAttestation/teaTutor`,
      }
    }
  },
  {
    path: "/LearningResultView",//管理者查看學生學習成果
    name: "LearningResultView",
    component: () => import("../views/LearningResultView.vue"),
    props: {
      userStatic: {
        data_structure: "LearningResultView",
        form_component: "",
        interface: "teaconsult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        get_data:`${Vue.prototype.$apiroot}/TeaAttestation/LearningResult`,
      }
    }
  },
  {
    path: "/MultipleLearningView",//管理者查看學生多元表現
    name: "MultipleLearningView",
    component: () => import("../views/MultipleLearningView.vue"),
    props: {
      userStatic: {
        data_structure: "MultipleLearningView",
        form_component: "",
        interface: "teaconsult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        get_data:`${Vue.prototype.$apiroot}/TeaAttestation/MultipleLearning`,
      }
    }
  },
  {
    path: "/AttestationNotYet",//管理者查看教師末證數
    name: "AttestationNotYet",
    component: () => import("../components/admin/AttestationNotYet.vue"),
    props: {
      userStatic: {
        data_structure: "AttestationNotYet",
        form_component: "",
        interface: "teaconsult",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        get_data:`${Vue.prototype.$apiroot}/TeaAttestation/AttestationNotYet`,
        get_stddata:`${Vue.prototype.$apiroot}/TeaAttestation/AttestationStdNotYet`,
      }
    }
  },
  {
    path: "/SystemSetup",//設定功能參數
    name: "SystemSetup",
    component: () => import("../components/admin/SystemSetUp.vue"),
    props: {
      userStatic: {
        data_structure: "SystemSetup",
        form_component: "",
        interface: "SystemSetup",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        save_data:`${Vue.prototype.$apiroot}/Update_L01_setup`,
        get_data:`${Vue.prototype.$apiroot}/Get_L01_setup`,
      }
    }
  },
  {
    path: "/OperateSetUp",//設定學校作業時間
    name: "OperateSetUp",
    component: () => import("../views/OperateView.vue"),
    props: {
      userStatic: {
        data_structure: "OperateSetUp",
        form_component: "",
        interface: "OpSetUp",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        save_data:`${Vue.prototype.$apiroot}/Update_L01_operate`,
        get_data:`${Vue.prototype.$apiroot}/Get_L01_operate`,
      }
    }
  },
  {
    path: "/StuTurnView",//學習歷程資料匯出入
    name: "StuTurnView",
    component: () => import("../views/StuTurnView.vue"),
    props: {
      userStatic: {
        data_structure: "StuTurnExport",
        form_component: "",
        interface: "StuTurnView",
        file_delete:true,
        file_download:true,
      },
      api_interface:
      {
        import_file:`${Vue.prototype.$apiroot}/StuTurn/Import`,
        list_file:`${Vue.prototype.$apiroot}/StuTurn/ExportFileList`,
        export_file:`${Vue.prototype.$apiroot}/StuTurn/ExportFile`,
        export_data:`${Vue.prototype.$apiroot}/StuTurn/Export`,
        get_studata:`${Vue.prototype.$apiroot}/S04_student`,
      }
    }
  },
]


const router = new VueRouter({
  routes,
})

router.beforeEach((to, from, next) => {
  next();
  router.app.$nextTick(() => {
    // document.body.scrollTop = 0;
  });
});

export default router
