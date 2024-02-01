using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.TeaAttestation.DbModels;
using StudentLearningHistory.Models.TeaAttestation.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuCollege.Parameters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
//using StudentLearningHistory.Models.TeaConsult.DbModels;

namespace StudentLearningHistory.Services
{
	public class TeaAttestationService
	{
		private readonly IDapperContext _context;
		private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public class GetEmpId
        {
            public string emp_id { get; set; }
        }
        public TeaAttestationService(IDapperContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
			_schno = _context.SchNo;
		}

		public async Task<IEnumerable<HeaderList>> GetList(StuAttestationQueryList arg)
		{
			arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *,
                                    (select count(*)
                                    from L01_std_public_filehub a
                                    where a.class_name = 'StuAttestation'
									and	  a.number_id = NewTable.i
                                    and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
									x_total =
									(select count(*)
									 from s04_norstusc a,s04_noropenc b
									  where	convert(varchar,NewTable.b)= a.in_year and 
											convert(varchar,NewTable.c) = a.in_sms and 
											NewTable.as_std_no = a.std_no and 
											a.noroc_id = b.noroc_id and
											a.stu_status in(1) and
											isnull(b.course_code,'') <> '' and
											isnull(b.is_learn,'N') = 'Y')
									+
									(select count(*)
									 from s04_norstusc a,s04_noropenc b
									  where convert(varchar,NewTable.b)= b.year_id and 
													convert(varchar,NewTable.c) = b.sms_id and 
													NewTable.as_std_no = a.std_no and 
													a.noroc_id = b.noroc_id and
													a.stu_status in(2,3))
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
										from (
										SELECT 
												as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
												as_std_no = convert(varchar(10),s04_student.std_no),			--學號
												as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
												as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
													convert(varchar(1),s04_noropenc.dep_id) + 
													case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
													convert(varchar(1),s04_noropenc.grd_id) + 
													case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
													convert(varchar(1),s04_noropenc.dep_id) + 
													case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
													convert(varchar(1),s04_noropenc.grd_id) + 
													case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
												as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
												as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
												as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
												as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
												as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
												as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
												as_o_syear =null, --原(應)修課學年度
												as_o_sems = null, --原(應)修課學期
												as_o_cls_year = null, --原(應)開課年級
												as_o_cls_no = null, --原(應)開課班級代碼
												as_o_cls_name = null, --原(應)開課班級名稱
												as_o_cls_group = null, --原(應)開課班級內分組
												as_o_subj_id = null, --原(應)修校務系統科目內碼
												as_o_course_id = null , --原(應)修課程代碼
												as_o_subj_name = null, --原(應)修科目名稱
												as_o_credits = null, --原(應)修學分
												as_makeup_mode = null, --補修方式
												as_rebuild_mode = null, --重修方式
												as_is_precourse = '0', --是否為預選課程
												as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
												as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
    											as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
													convert(varchar(1),s04_noropenc.dep_id) + 
													case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
													convert(varchar(1),s04_noropenc.grd_id) + 
													case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												e = convert(varchar(10),s04_subject.sub_id),
												f = convert(varchar(23),s04_stddbgo.course_code),
												g = convert(varchar(50),s04_noropenc.all_empid),
												h = s04_student.std_no ,
												a.ser_id as i,
												s04_noropenc.sub_credit as j,
												convert(varchar(1),s04_norstusc.stu_status) as k,
												a.attestation_send as l,
												a.attestation_date as m,
												a.attestation_status as n,
												a.is_sys as o,
												a.content as p,
												convert(varchar(90),s04_108subject.sub108_name) as q,
												convert(varchar(50),s04_noropenc.all_empname) as r,
												s = 
												(
												Select SUBSTRING(
													(Select ','+ file_name
													From L01_std_public_filehub b
												where 
														b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
														b.class_name = 'StuAttestation' 
													For Xml Path(''))
												, 2, 8000)
												)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id and
												s04_student.std_no = @std_no
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
											inner join s04_norstusc on
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.in_year = s04_stuhcls.year_id and
												s04_norstusc.in_sms = s04_stuhcls.sms_id and
												s04_norstusc.stu_status = 1 --新修
											inner join s04_noropenc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												isnull(s04_noropenc.course_code,'') <> '' and
												isnull(s04_noropenc.is_learn,'N') = 'Y'
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s04_stddbgo on
												s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
												s04_stuhcls.sms_id = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_stuhcls.grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											left join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											left join s90_branch on
												s04_noropenc.bra_id = s90_branch.bra_id
											left join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_hstusubjscoreterm.hstusst_status = 1
											left join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
											left join L01_std_attestation a on
												a.year_id = s04_stuhcls.year_id and
												a.sms_id = s04_stuhcls.sms_id and
												a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
														convert(varchar(1),s04_noropenc.dep_id) + 
														case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
														convert(varchar(1),s04_noropenc.grd_id) + 
														case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
												a.sub_id = convert(varchar(10),s04_subject.sub_id) and
												a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
												a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
											where s04_student.std_hisdatayear >= 108
										union
										SELECT 
												as_id_no = convert(varchar(10),s04_student.std_identity) ,
												as_std_no = convert(varchar(10),s04_student.std_no),
												as_grade = convert(smallint,s04_stuhcls.grd_id) ,
												as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
												as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
												as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
												as_subj_uid = convert(varchar(10),s04_subject.sub_id),
												as_course_id = convert(varchar(23),s04_stddbgo.course_code),
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
												as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
												as_type = convert(varchar(1),s04_norstusc.stu_status),
												as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
												as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
												as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
												as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
												as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
													case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
													convert(varchar(1),noro_o.grd_id) + 
													case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
												 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
												as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
												as_o_subj_id = convert(varchar(10),noro_o.sub_id),
												as_o_course_id = convert(varchar(23),noro_o.course_code),
												as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
												as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
												as_makeup_mode = null,
												as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
												as_is_precourse = '0',
												as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
												as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
    											as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												d = convert(varchar(6),s04_noropenc.noroc_id),
												e = convert(varchar(10),s04_subject.sub_id),
												f = convert(varchar(23),s04_stddbgo.course_code),
												g = convert(varchar(50),s04_noropenc.all_empid),
												h = s04_student.std_no ,
												a.ser_id as i,
												s04_noropenc.sub_credit as j,
												convert(varchar(1),s04_norstusc.stu_status) as k,
												a.attestation_send as l,
												a.attestation_date as m,
												a.attestation_status as n,
												a.is_sys as o,
												a.content as p,
												convert(varchar(90),s04_108subject.sub108_name) as q,
												convert(varchar(50),s04_noropenc.all_empname) as r,
												s = 
												(
												Select SUBSTRING(
													(Select ','+ file_name
													From L01_std_public_filehub b
												where 
														b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
														b.class_name = 'StuAttestation' 
													For Xml Path(''))
												, 2, 8000)
												)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id and
												s04_student.std_no = @std_no
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
											inner join s04_noropenc on
												s04_noropenc.year_id = s04_stuhcls.year_id and
												s04_noropenc.sms_id = s04_stuhcls.sms_id
											inner join s04_norstusc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.stu_status = 2 --重修
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
												s04_hstusubjscoreterm.hstusst_status = 2
											inner join s04_stddbgo on
												s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
												s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											inner join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											inner join s90_branch on
												s04_noropenc.bra_id = s90_branch.bra_id
											inner join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
											inner join s04_hstusubjscoreterm hstusst_new on
												s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
												s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
												s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
												s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
												hstusst_new.hstusst_status = 1
											inner join s04_norstusc norstu_o on
												hstusst_new.std_no = norstu_o.std_no and
												hstusst_new.in_year = norstu_o.in_year and
												hstusst_new.in_sms = norstu_o.in_sms and
												norstu_o.stu_status = 1
											inner join s04_noropenc noro_o on
												norstu_o.noroc_id = noro_o.noroc_id and
												hstusst_new.sub_id = noro_o.sub_id 		
											inner join s90_organization org_o on
												noro_o.org_id = org_o.org_id
											left join L01_std_attestation a on
												a.year_id = s04_stuhcls.year_id and
												a.sms_id = s04_stuhcls.sms_id and
												a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
												a.sub_id = convert(varchar(10),s04_subject.sub_id) and
												a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
												a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
											where s04_student.std_hisdatayear >= 108 
										union
										SELECT 
												as_id_no = convert(varchar(10),s04_student.std_identity) ,
												as_std_no = convert(varchar(10),s04_student.std_no),
												as_grade = convert(smallint,s04_stuhcls.grd_id) ,
												as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
												as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
												as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
												as_subj_uid = convert(varchar(10),s04_subject.sub_id),
												as_course_id = convert(varchar(23),s04_stddbgo.course_code),
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
												as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
												as_type = convert(varchar(1),s04_norstusc.stu_status),
												as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
												as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
												as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
												as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
												as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
													case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
													convert(varchar(1),s04_stddbgo.grd_id) + 
													case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
												as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
												as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
												as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
												as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
												as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
												as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
												as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
												as_rebuild_mode = null,
												as_is_precourse = '0',
												as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
												as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
    											as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												d = convert(varchar(6),s04_noropenc.noroc_id),
												e = convert(varchar(10),s04_subject.sub_id),
												f = convert(varchar(23),s04_stddbgo.course_code),
												g = convert(varchar(50),s04_noropenc.all_empid),
												h = s04_student.std_no ,
												a.ser_id as i,
												s04_noropenc.sub_credit as j,
												convert(varchar(1),s04_norstusc.stu_status) as k,
												a.attestation_send as l,
												a.attestation_date as m,
												a.attestation_status as n,
												a.is_sys as o,
												a.content as p,
												convert(varchar(90),s04_108subject.sub108_name) as q,
												convert(varchar(50),s04_noropenc.all_empname) as r,
												s = 
												(
												Select SUBSTRING(
													(Select ','+ file_name
													From L01_std_public_filehub b
												where 
														b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
														b.class_name = 'StuAttestation' 
													For Xml Path(''))
												, 2, 8000)
												)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id and
												s04_student.std_no = @std_no
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
											inner join s04_noropenc on
												s04_noropenc.year_id = s04_stuhcls.year_id and
												s04_noropenc.sms_id = s04_stuhcls.sms_id 
											inner join s04_norstusc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.stu_status = 3 --補修
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											inner join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
												s04_hstusubjscoreterm.hstusst_status = 3
											left join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
											inner join s04_stddbgo on
												s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
												s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											inner join s04_ytdbgoc on
												s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
												s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
												s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
												s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
												s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
												s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
												s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
											inner join s90_organization org_o on
												s04_ytdbgoc.org_id = org_o.org_id
											inner join s90_branch on
												s04_ytdbgoc.bra_id = s90_branch.bra_id
											inner join s04_108subject subj108_o on
												s04_stddbgo.course_code = subj108_o.course_code
											left join L01_std_attestation a on
												a.year_id = s04_stuhcls.year_id and
												a.sms_id = s04_stuhcls.sms_id and
												a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
												a.sub_id = convert(varchar(10),s04_subject.sub_id) and
												a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
												a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
											where s04_student.std_hisdatayear >= 108
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by as_cls_uid,as_course_id";

			using (IDbConnection conn = _context.CreateCommand())
			{
				return await conn.QueryAsync<HeaderList>(str_sql, arg);
			}
		}

		public async Task<IEnumerable<HeaderList>> GetListConfirm(StuAttestationQueryList arg)
		{
			arg.sch_no = _context.SchNo;
            #region old sql
            //        string str_sql = @"
            //                      SELECT *,
            //                          (select count(*)
            //                          from L01_std_public_filehub a
            //                          where a.class_name = 'StuAttestation'
            //                          and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
            //						x_total =
            //									(select count(*)
            //										from L01_std_attestation a
            //										where a.sch_no = NewTable.a
            //										and a.year_id = NewTable.b
            //										and a.sms_id = NewTable.c
            //										and a.emp_id = NewTable.g
            //										and isnull(a.attestation_send,'') <>'' 
            //										),
            //						'' as x_status
            //                      FROM(
            //select *,
            //		ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid,h) AS RowNum
            //from (
            //SELECT 
            //		as_name = s04_student.std_name,
            //		as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
            //		as_std_no = convert(varchar(10),s04_student.std_no),			--學號
            //		as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
            //		as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //			convert(varchar(1),s04_noropenc.dep_id) + 
            //			case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //			convert(varchar(1),s04_noropenc.grd_id) + 
            //			case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //		 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
            //			convert(varchar(1),s04_noropenc.dep_id) + 
            //			case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //			convert(varchar(1),s04_noropenc.grd_id) + 
            //			case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //		as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
            //		as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
            //		as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
            //		as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
            //		as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
            //		as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
            //		as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
            //		as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
            //		as_o_syear =null, --原(應)修課學年度
            //		as_o_sems = null, --原(應)修課學期
            //		as_o_cls_year = null, --原(應)開課年級
            //		as_o_cls_no = null, --原(應)開課班級代碼
            //		as_o_cls_name = null, --原(應)開課班級名稱
            //		as_o_cls_group = null, --原(應)開課班級內分組
            //		as_o_subj_id = null, --原(應)修校務系統科目內碼
            //		as_o_course_id = null , --原(應)修課程代碼
            //		as_o_subj_name = null, --原(應)修科目名稱
            //		as_o_credits = null, --原(應)修學分
            //		as_makeup_mode = null, --補修方式
            //		as_rebuild_mode = null, --重修方式
            //		as_is_precourse = '0', --是否為預選課程
            //		as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
            //		as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
            //					as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
            //		s04_stuhcls.sch_no as a,
            //		s04_stuhcls.year_id as b,
            //		s04_stuhcls.sms_id as c,
            //		d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //			convert(varchar(1),s04_noropenc.dep_id) + 
            //			case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //			convert(varchar(1),s04_noropenc.grd_id) + 
            //			case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
            //		e = convert(varchar(10),s04_subject.sub_id),
            //		f = convert(varchar(23),s04_stddbgo.course_code),
            //		g = convert(varchar(50),s04_noropenc.all_empid),
            //		h = s04_student.std_no ,
            //		a.ser_id as i,
            //		s04_noropenc.sub_credit as j,
            //		convert(varchar(1),s04_norstusc.stu_status) as k,
            //		a.attestation_send as l,
            //		a.attestation_date as m,
            //		a.attestation_status as n,
            //		a.is_sys as o,
            //		a.content as p,
            //		convert(varchar(90),s04_108subject.sub108_name) as q,
            //		convert(varchar(50),s04_noropenc.all_empname) as r,
            //		s = 
            //		(
            //		Select SUBSTRING(
            //			(Select ','+ file_name
            //			From L01_std_public_filehub b
            //		where 
            //				b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
            //				b.class_name = 'StuAttestation' 
            //			For Xml Path(''))
            //		, 2, 8000)
            //		),
            //		u = a.attestation_confirm,
            //		v = a.attestation_release,
            //		w = a.attestation_reason
            //	FROM s04_student
            //	inner join s04_stuhcls on
            //		s04_student.std_no = s04_stuhcls.std_no and												
            //		s04_stuhcls.sch_no = @sch_no and 
            //		s04_stuhcls.year_id = @year_id and 
            //		s04_stuhcls.sms_id = @sms_id 
            //	inner join s90_cha_id on
            //		s04_stuhcls.std_status = s90_cha_id.cha_id and
            //		s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //	inner join s04_norstusc on
            //		s04_student.std_no = s04_norstusc.std_no and
            //		s04_norstusc.in_year = s04_stuhcls.year_id and
            //		s04_norstusc.in_sms = s04_stuhcls.sms_id and
            //		s04_norstusc.stu_status = 1 --新修
            //	inner join s04_noropenc on
            //		s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //		isnull(s04_noropenc.course_code,'') <> '' and
            //		isnull(s04_noropenc.is_learn,'N') = 'Y' and
            //		charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
            //	inner join s04_subject on
            //		s04_noropenc.sub_id = s04_subject.sub_id
            //	inner join s04_stddbgo on
            //		s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
            //		s04_stuhcls.sms_id = s04_stddbgo.sms_id and
            //		s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //		s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //		s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //		s04_stuhcls.grd_id = s04_stddbgo.grd_id and
            //		s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //		isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //		s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //		( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //	inner join s04_108subject on
            //		s04_stddbgo.course_code = s04_108subject.course_code
            //	inner join L01_std_attestation a on
            //		a.year_id = s04_stuhcls.year_id and
            //		a.sms_id = s04_stuhcls.sms_id and
            //		a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
            //				convert(varchar(1),s04_noropenc.dep_id) + 
            //				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
            //				convert(varchar(1),s04_noropenc.grd_id) + 
            //				case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
            //		a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //		a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //		a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
            //		a.std_no = s04_student.std_no and
            //		isnull(a.attestation_send,'') <>'' 
            //	left join s90_organization on
            //		s04_noropenc.org_id = s90_organization.org_id
            //	left join s90_branch on
            //		s04_noropenc.bra_id = s90_branch.bra_id
            //	left join s04_hstusubjscoreterm on
            //		s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //		s04_hstusubjscoreterm.hstusst_status = 1
            //	left join s04_hstusixscoreterm on
            //		s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //	left join s04_stubuterm on
            //		s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //	where s04_student.std_hisdatayear >= 108
            //union
            //SELECT 
            //		as_name = s04_student.std_name,
            //		as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //		as_std_no = convert(varchar(10),s04_student.std_no),
            //		as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //		as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //		as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //		as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //		as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //		as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //		as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //		as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //		as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //		as_type = convert(varchar(1),s04_norstusc.stu_status),
            //		as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //		as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
            //		as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
            //		as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
            //		as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
            //			case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
            //			convert(varchar(1),noro_o.grd_id) + 
            //			case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
            //		 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
            //		as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //		as_o_subj_id = convert(varchar(10),noro_o.sub_id),
            //		as_o_course_id = convert(varchar(23),noro_o.course_code),
            //		as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //		as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
            //		as_makeup_mode = null,
            //		as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //		as_is_precourse = '0',
            //		as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //		as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //					as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //		s04_stuhcls.sch_no as a,
            //		s04_stuhcls.year_id as b,
            //		s04_stuhcls.sms_id as c,
            //		d = convert(varchar(6),s04_noropenc.noroc_id),
            //		e = convert(varchar(10),s04_subject.sub_id),
            //		f = convert(varchar(23),s04_stddbgo.course_code),
            //		g = convert(varchar(50),s04_noropenc.all_empid),
            //		h = s04_student.std_no ,
            //		a.ser_id as i,
            //		s04_noropenc.sub_credit as j,
            //		convert(varchar(1),s04_norstusc.stu_status) as k,
            //		a.attestation_send as l,
            //		a.attestation_date as m,
            //		a.attestation_status as n,
            //		a.is_sys as o,
            //		a.content as p,
            //		convert(varchar(90),s04_108subject.sub108_name) as q,
            //		convert(varchar(50),s04_noropenc.all_empname) as r,
            //		s = 
            //		(
            //		Select SUBSTRING(
            //			(Select ','+ file_name
            //			From L01_std_public_filehub b
            //		where 
            //				b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
            //				b.class_name = 'StuAttestation' 
            //			For Xml Path(''))
            //		, 2, 8000)
            //		),
            //		u = a.attestation_confirm,
            //		v = a.attestation_release,
            //		w = a.attestation_reason
            //	FROM s04_student
            //	inner join s04_stuhcls on
            //		s04_student.std_no = s04_stuhcls.std_no and												
            //		s04_stuhcls.sch_no = @sch_no and 
            //		s04_stuhcls.year_id = @year_id and 
            //		s04_stuhcls.sms_id = @sms_id
            //	inner join s90_cha_id on
            //		s04_stuhcls.std_status = s90_cha_id.cha_id and
            //		s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
            //	inner join s04_noropenc on
            //		s04_noropenc.year_id = s04_stuhcls.year_id and
            //		s04_noropenc.sms_id = s04_stuhcls.sms_id  and
            //		charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
            //	inner join s04_norstusc on
            //		s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //		s04_student.std_no = s04_norstusc.std_no and
            //		s04_norstusc.stu_status = 2 --重修
            //	inner join s04_subject on
            //		s04_noropenc.sub_id = s04_subject.sub_id
            //	inner join s04_hstusubjscoreterm on
            //		s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //		s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //		s04_hstusubjscoreterm.hstusst_status = 2
            //	inner join s04_stddbgo on
            //		s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //		s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //		s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //		s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //		s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //		s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //		s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //		isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //		s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //		( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //	inner join s04_108subject on
            //		s04_stddbgo.course_code = s04_108subject.course_code
            //	inner join s90_organization on
            //		s04_noropenc.org_id = s90_organization.org_id
            //	inner join s90_branch on
            //		s04_noropenc.bra_id = s90_branch.bra_id
            //	inner join s04_hstusixscoreterm on
            //		s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //	 join L01_std_attestation a on
            //		a.year_id = s04_stuhcls.year_id and
            //		a.sms_id = s04_stuhcls.sms_id and
            //		a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //		a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //		a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //		a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
            //		a.std_no = s04_student.std_no and
            //		isnull(a.attestation_send,'') <>'' 
            //	left join s04_stubuterm on
            //		s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
            //	inner join s04_hstusubjscoreterm hstusst_new on
            //		s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
            //		s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
            //		s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
            //		s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
            //		hstusst_new.hstusst_status = 1
            //	inner join s04_norstusc norstu_o on
            //		hstusst_new.std_no = norstu_o.std_no and
            //		hstusst_new.in_year = norstu_o.in_year and
            //		hstusst_new.in_sms = norstu_o.in_sms and
            //		norstu_o.stu_status = 1
            //	inner join s04_noropenc noro_o on
            //		norstu_o.noroc_id = noro_o.noroc_id and
            //		hstusst_new.sub_id = noro_o.sub_id 		
            //	inner join s90_organization org_o on
            //		noro_o.org_id = org_o.org_id
            //	where s04_student.std_hisdatayear >= 108 
            //union
            //SELECT 
            //		as_name = s04_student.std_name,
            //		as_id_no = convert(varchar(10),s04_student.std_identity) ,
            //		as_std_no = convert(varchar(10),s04_student.std_no),
            //		as_grade = convert(smallint,s04_stuhcls.grd_id) ,
            //		as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
            //		as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
            //		as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
            //		as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
            //		as_subj_uid = convert(varchar(10),s04_subject.sub_id),
            //		as_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //		as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
            //		as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
            //		as_type = convert(varchar(1),s04_norstusc.stu_status),
            //		as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
            //		as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
            //		as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
            //		as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
            //		as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
            //			case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
            //			convert(varchar(1),s04_stddbgo.grd_id) + 
            //			case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
            //		as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
            //		as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
            //		as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
            //		as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
            //		as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
            //		as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
            //		as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
            //		as_rebuild_mode = null,
            //		as_is_precourse = '0',
            //		as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
            //		as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
            //					as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
            //		s04_stuhcls.sch_no as a,
            //		s04_stuhcls.year_id as b,
            //		s04_stuhcls.sms_id as c,
            //		d = convert(varchar(6),s04_noropenc.noroc_id),
            //		e = convert(varchar(10),s04_subject.sub_id),
            //		f = convert(varchar(23),s04_stddbgo.course_code),
            //		g = convert(varchar(50),s04_noropenc.all_empid),
            //		h = s04_student.std_no ,
            //		a.ser_id as i,
            //		s04_noropenc.sub_credit as j,
            //		convert(varchar(1),s04_norstusc.stu_status) as k,
            //		a.attestation_send as l,
            //		a.attestation_date as m,
            //		a.attestation_status as n,
            //		a.is_sys as o,
            //		a.content as p,
            //		convert(varchar(90),s04_108subject.sub108_name) as q,
            //		convert(varchar(50),s04_noropenc.all_empname) as r,
            //		s = 
            //		(
            //		Select SUBSTRING(
            //			(Select ','+ file_name
            //			From L01_std_public_filehub b
            //		where 
            //				b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
            //				b.class_name = 'StuAttestation' 
            //			For Xml Path(''))
            //		, 2, 8000)
            //		),
            //		u = a.attestation_confirm,
            //		v = a.attestation_release,
            //		w = a.attestation_reason
            //	FROM s04_student
            //	inner join s04_stuhcls on
            //		s04_student.std_no = s04_stuhcls.std_no and												
            //		s04_stuhcls.sch_no = @sch_no and 
            //		s04_stuhcls.year_id = @year_id and 
            //		s04_stuhcls.sms_id = @sms_id 
            //	inner join s90_cha_id on
            //		s04_stuhcls.std_status = s90_cha_id.cha_id and
            //		s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
            //	inner join s04_noropenc on
            //		s04_noropenc.year_id = s04_stuhcls.year_id and
            //		s04_noropenc.sms_id = s04_stuhcls.sms_id  and
            //		charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
            //	inner join s04_norstusc on
            //		s04_norstusc.noroc_id = s04_noropenc.noroc_id and
            //		s04_student.std_no = s04_norstusc.std_no and
            //		s04_norstusc.stu_status = 3 --補修
            //	inner join s04_subject on
            //		s04_noropenc.sub_id = s04_subject.sub_id
            //	inner join s90_organization on
            //		s04_noropenc.org_id = s90_organization.org_id
            //	inner join s04_hstusubjscoreterm on
            //		s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
            //		s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
            //		s04_hstusubjscoreterm.hstusst_status = 3
            //	left join s04_hstusixscoreterm on
            //		s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
            //		s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
            //		s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
            //	left join s04_stubuterm on
            //		s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
            //		s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
            //		s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
            //		s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
            //	inner join s04_stddbgo on
            //		s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
            //		s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
            //		s04_stuhcls.deg_id = s04_stddbgo.deg_id and
            //		s04_stuhcls.dep_id = s04_stddbgo.dep_id and
            //		s04_stuhcls.bra_id = s04_stddbgo.bra_id and
            //		s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
            //		s04_stuhcls.cg_code = s04_stddbgo.cg_code and
            //		isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
            //		s04_noropenc.sub_id = s04_stddbgo.sub_id and
            //		( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
            //	inner join s04_108subject on
            //		s04_stddbgo.course_code = s04_108subject.course_code
            //	inner join s04_ytdbgoc on
            //		s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
            //		s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
            //		s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
            //		s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
            //		s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
            //		s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
            //		s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
            //	inner join s90_organization org_o on
            //		s04_ytdbgoc.org_id = org_o.org_id
            //	inner join s90_branch on
            //		s04_ytdbgoc.bra_id = s90_branch.bra_id
            //	inner join s04_108subject subj108_o on
            //		s04_stddbgo.course_code = subj108_o.course_code
            //	 join L01_std_attestation a on
            //		a.year_id = s04_stuhcls.year_id and
            //		a.sms_id = s04_stuhcls.sms_id and
            //		a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
            //		a.sub_id = convert(varchar(10),s04_subject.sub_id) and
            //		a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
            //		a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
            //		a.std_no = s04_student.std_no and
            //		isnull(a.attestation_send,'') <>'' 
            //	where s04_student.std_hisdatayear >= 108
            //	) a
            //                          ) AS NewTable
            //                      WHERE RowNum >= 1 AND RowNum <= 999
            //                      order by as_cls_uid,as_course_id,h";
            #endregion

            string str_sql = @"
												SELECT *,
												x_cnt = 
													(select count(*)
													 from L01_std_public_filehub a
													 where a.class_name = 'StuAttestation'
													and	  a.number_id = NewTable.i
													 and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
												x_total =
														(select count(*)
														 from L01_std_attestation a
														 where a.sch_no = NewTable.a
														 and a.year_id = NewTable.b
														 and a.sms_id = NewTable.c
														 and a.emp_id = NewTable.g
														 and isnull(a.attestation_send,'') <>'' 
														),
												x_status = ''
												FROM(
												select *,
														ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e,a.h) AS RowNum
												from (
														select 
														a = L01_std_attestation.sch_no,
														b = L01_std_attestation.year_id,
														c = L01_std_attestation.sms_id ,
														d = L01_std_attestation.cls_id,
														e = L01_std_attestation.sub_id,
														f = L01_std_attestation.src_dup,
														g = L01_std_attestation.emp_id,
														h = L01_std_attestation.std_no,
														i = L01_std_attestation.ser_id,
														j = L01_std_attestation.credit,
														k = L01_std_attestation.stu_status,
														l = L01_std_attestation.attestation_send,
														m = L01_std_attestation.attestation_date,
														n = L01_std_attestation.attestation_status,
														o = L01_std_attestation.is_sys,
														p = L01_std_attestation.content,
														q = L01_std_attestation.sub_name,
														r = L01_std_attestation.all_empname,
														s = 
														(
														Select SUBSTRING(
															(Select ','+ file_name
																From L01_std_public_filehub b
																where 
																b.complex_key = L01_std_attestation.sch_no+'_'+
																				convert(varchar,L01_std_attestation.year_id)+'_'+
																				convert(varchar,L01_std_attestation.sms_id)+'_'+
																				L01_std_attestation.cls_id+'_'+
																				L01_std_attestation.sub_id+'_'+
																				L01_std_attestation.src_dup+'_'+
																				L01_std_attestation.emp_id+'_'+
																				L01_std_attestation.std_no+'_0' and
																b.class_name = 'StuAttestation' 
															For Xml Path(''))
														, 2, 8000)
														),
														u = L01_std_attestation.attestation_confirm,
														v = L01_std_attestation.attestation_release,
														w = L01_std_attestation.attestation_reason,
														as_cls_name = L01_std_attestation.cls_name,
														as_name = std_name,
														as_cur_teaname = L01_std_attestation.all_empname,
														as_subj_uid = L01_std_attestation.sub_id,
														as_cls_uid = L01_std_attestation.cls_id,
														as_std_no = L01_std_attestation.std_no,
														as_subj_name = L01_std_attestation.sub_name
														from L01_std_attestation
														join s04_student on s04_student.std_no = L01_std_attestation.std_no
														join s04_stuhcls on s04_stuhcls.year_id = L01_std_attestation.year_id and
														s04_stuhcls.sms_id = L01_std_attestation.sms_id and
														s04_stuhcls.std_no = L01_std_attestation.std_no and
														s04_stuhcls.grd_id = @grade_id
														where L01_std_attestation.sch_no = @sch_no  
														and L01_std_attestation.year_id = @year_id  
														and L01_std_attestation.sms_id = @sms_id 
														and charindex(@emp_id,convert(varchar(50),emp_id)) > 0
														and isnull(L01_std_attestation.attestation_send,'') <>'' 
													)a
												) AS NewTable
												WHERE RowNum >= 1 AND RowNum <= 999
												order by d,e,h";
            string str_sql_emp = @"
                                                        select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
			{
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
			}
		}

		public async Task<IEnumerable<HeaderList>> GetListRelease(StuAttestationQueryList arg)
		{
			arg.sch_no = _context.SchNo;
			#region old sql
			//   string str_sql = @"
			//                       SELECT *,
			//                           (select count(*)
			//                           from L01_std_public_filehub a
			//                           where a.class_name = 'StuAttestation'
			//                           and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0') as x_cnt,
			//x_total =
			//(select count(*)
			// from L01_std_attestation a
			//	where a.sch_no = NewTable.a
			//	and a.year_id = NewTable.b
			//	and a.sms_id = NewTable.c
			//	and a.emp_id = NewTable.g
			//	and isnull(a.attestation_send,'') <>'' 
			//	and isnull(a.attestation_confirm,'') = 'Y' 
			//  )
			//                       FROM(
			//	select *,
			//			ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
			//	from (
			//	SELECT 
			//			as_name = s04_student.std_name,
			//			as_id_no = convert(varchar(10),s04_student.std_identity) ,		--身份證字號
			//			as_std_no = convert(varchar(10),s04_student.std_no),			--學號
			//			as_grade = convert(smallint,s04_stuhcls.grd_id) ,				--學生年級
			//			as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//				convert(varchar(1),s04_noropenc.dep_id) + 
			//				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//				convert(varchar(1),s04_noropenc.grd_id) + 
			//				case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//			 as_cls_no = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級代碼
			//				convert(varchar(1),s04_noropenc.dep_id) + 
			//				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//				convert(varchar(1),s04_noropenc.grd_id) + 
			//				case when s04_noropenc.cls_id >= 10 then '0' + convert(varchar(2),s04_noropenc.cls_id) else '00' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
			//			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end), --開課班級內分組
			//			as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
			//			as_course_id = convert(varchar(23),s04_stddbgo.course_code), --課程計畫平臺核發之課程代碼	
			//			as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
			//			as_credits = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end), --學分/時數
			//			as_type = convert(varchar(1),s04_norstusc.stu_status),	--修課類別
			//			as_use_credits = convert(varchar(1),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ), --是否採計學分
			//			as_o_syear =null, --原(應)修課學年度
			//			as_o_sems = null, --原(應)修課學期
			//			as_o_cls_year = null, --原(應)開課年級
			//			as_o_cls_no = null, --原(應)開課班級代碼
			//			as_o_cls_name = null, --原(應)開課班級名稱
			//			as_o_cls_group = null, --原(應)開課班級內分組
			//			as_o_subj_id = null, --原(應)修校務系統科目內碼
			//			as_o_course_id = null , --原(應)修課程代碼
			//			as_o_subj_name = null, --原(應)修科目名稱
			//			as_o_credits = null, --原(應)修學分
			//			as_makeup_mode = null, --補修方式
			//			as_rebuild_mode = null, --重修方式
			//			as_is_precourse = '0', --是否為預選課程
			//			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid), --授課教師內碼
			//			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode), --授課教師代碼
			//						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname), --授課教師姓名
			//			s04_stuhcls.sch_no as a,
			//			s04_stuhcls.year_id as b,
			//			s04_stuhcls.sms_id as c,
			//			d = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//				convert(varchar(1),s04_noropenc.dep_id) + 
			//				case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//				convert(varchar(1),s04_noropenc.grd_id) + 
			//				case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
			//			e = convert(varchar(10),s04_subject.sub_id),
			//			f = convert(varchar(23),s04_stddbgo.course_code),
			//			g = convert(varchar(50),s04_noropenc.all_empid),
			//			h = s04_student.std_no ,
			//			a.ser_id as i,
			//			s04_noropenc.sub_credit as j,
			//			convert(varchar(1),s04_norstusc.stu_status) as k,
			//			a.attestation_send as l,
			//			a.attestation_date as m,
			//			a.attestation_status as n,
			//			a.is_sys as o,
			//			a.content as p,
			//			convert(varchar(90),s04_108subject.sub108_name) as q,
			//			convert(varchar(50),s04_noropenc.all_empname) as r,
			//			s = 
			//			(
			//			Select SUBSTRING(
			//				(Select ','+ file_name
			//				From L01_std_public_filehub b
			//			where 
			//					b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
			//					b.class_name = 'StuAttestation' 
			//				For Xml Path(''))
			//			, 2, 8000)
			//			),
			//			u = a.attestation_confirm,
			//			v = a.attestation_release,
			//			w = a.attestation_reason
			//		FROM s04_student
			//		inner join s04_stuhcls on
			//			s04_student.std_no = s04_stuhcls.std_no and												
			//			s04_stuhcls.sch_no = @sch_no and 
			//			s04_stuhcls.year_id = @year_id and 
			//			s04_stuhcls.sms_id = @sms_id
			//		inner join s90_cha_id on
			//			s04_stuhcls.std_status = s90_cha_id.cha_id and
			//			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
			//		inner join s04_norstusc on
			//			s04_student.std_no = s04_norstusc.std_no and
			//			s04_norstusc.in_year = s04_stuhcls.year_id and
			//			s04_norstusc.in_sms = s04_stuhcls.sms_id and
			//			s04_norstusc.stu_status = 1 --新修
			//		inner join s04_noropenc on
			//			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//			isnull(s04_noropenc.course_code,'') <> '' and
			//			isnull(s04_noropenc.is_learn,'N') = 'Y' and
			//			charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
			//		inner join s04_subject on
			//			s04_noropenc.sub_id = s04_subject.sub_id
			//		inner join s04_stddbgo on
			//			s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
			//			s04_stuhcls.sms_id = s04_stddbgo.sms_id and
			//			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//			s04_stuhcls.grd_id = s04_stddbgo.grd_id and
			//			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//			s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//		inner join s04_108subject on
			//			s04_stddbgo.course_code = s04_108subject.course_code
			//		inner join L01_std_attestation a on
			//			a.year_id = s04_stuhcls.year_id and
			//			a.sms_id = s04_stuhcls.sms_id and
			//			a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
			//					convert(varchar(1),s04_noropenc.dep_id) + 
			//					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
			//					convert(varchar(1),s04_noropenc.grd_id) + 
			//					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ) and
			//			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and 
			//			a.std_no = s04_student.std_no and
			//			isnull(a.attestation_send,'') <>'' and
			//			isnull(a.attestation_status,'') in('Y','F')
			//		left join s90_organization on
			//			s04_noropenc.org_id = s90_organization.org_id
			//		left join s90_branch on
			//			s04_noropenc.bra_id = s90_branch.bra_id
			//		left join s04_hstusubjscoreterm on
			//			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//			s04_hstusubjscoreterm.hstusst_status = 1
			//		left join s04_hstusixscoreterm on
			//			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//		left join s04_stubuterm on
			//			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
			//		where s04_student.std_hisdatayear >= 108
			//	union
			//	SELECT 
			//			as_name = s04_student.std_name,
			//			as_id_no = convert(varchar(10),s04_student.std_identity) ,
			//			as_std_no = convert(varchar(10),s04_student.std_no),
			//			as_grade = convert(smallint,s04_stuhcls.grd_id) ,
			//			as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
			//			as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
			//			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
			//			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
			//			as_subj_uid = convert(varchar(10),s04_subject.sub_id),
			//			as_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//			as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//			as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
			//			as_type = convert(varchar(1),s04_norstusc.stu_status),
			//			as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
			//			as_o_syear = convert(smallint , s04_hstusubjscoreterm.in_year ),
			//			as_o_seme = convert(smallint , s04_hstusubjscoreterm.in_sms ),
			//			as_o_cls_year = convert(varchar(1) , noro_o.grd_id ),
			//			as_o_cls_no = convert(varchar(1),noro_o.dep_id) + 
			//				case when noro_o.bra_id >= 10 then convert(varchar(2),noro_o.bra_id) else '0' + convert(varchar(1),noro_o.bra_id) end + 
			//				convert(varchar(1),noro_o.grd_id) + 
			//				case when noro_o.cls_id >= 100 then convert(varchar(3),noro_o.cls_id) when noro_o.cls_id >= 10 then '0' + convert(varchar(2),noro_o.cls_id) else '00' + convert(varchar(1),noro_o.cls_id) end,
			//			 as_o_cls_name = convert(varchar(90),noro_o.noroc_clsname),
			//			as_o_cls_group = convert(varchar(20),case when isnull(noro_o.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
			//			as_o_subj_id = convert(varchar(10),noro_o.sub_id),
			//			as_o_course_id = convert(varchar(23),noro_o.course_code),
			//			as_o_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//			as_o_credits = convert(smallint , s04_noropenc.sub_credit) ,
			//			as_makeup_mode = null,
			//			as_rebuild_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
			//			as_is_precourse = '0',
			//			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
			//			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
			//						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
			//			s04_stuhcls.sch_no as a,
			//			s04_stuhcls.year_id as b,
			//			s04_stuhcls.sms_id as c,
			//			d = convert(varchar(6),s04_noropenc.noroc_id),
			//			e = convert(varchar(10),s04_subject.sub_id),
			//			f = convert(varchar(23),s04_stddbgo.course_code),
			//			g = convert(varchar(50),s04_noropenc.all_empid),
			//			h = s04_student.std_no ,
			//			a.ser_id as i,
			//			s04_noropenc.sub_credit as j,
			//			convert(varchar(1),s04_norstusc.stu_status) as k,
			//			a.attestation_send as l,
			//			a.attestation_date as m,
			//			a.attestation_status as n,
			//			a.is_sys as o,
			//			a.content as p,
			//			convert(varchar(90),s04_108subject.sub108_name) as q,
			//			convert(varchar(50),s04_noropenc.all_empname) as r,
			//			s = 
			//			(
			//			Select SUBSTRING(
			//				(Select ','+ file_name
			//				From L01_std_public_filehub b
			//			where 
			//					b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
			//					b.class_name = 'StuAttestation' 
			//				For Xml Path(''))
			//			, 2, 8000)
			//			),
			//			u = a.attestation_confirm,
			//			v = a.attestation_release,
			//			w = a.attestation_reason	
			//		FROM s04_student
			//		inner join s04_stuhcls on
			//			s04_student.std_no = s04_stuhcls.std_no and												
			//			s04_stuhcls.sch_no = @sch_no and 
			//			s04_stuhcls.year_id = @year_id and 
			//			s04_stuhcls.sms_id = @sms_id 
			//		inner join s90_cha_id on
			//			s04_stuhcls.std_status = s90_cha_id.cha_id and
			//			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
			//		inner join s04_noropenc on
			//			s04_noropenc.year_id = s04_stuhcls.year_id and
			//			s04_noropenc.sms_id = s04_stuhcls.sms_id  and
			//			charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
			//		inner join s04_norstusc on
			//			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//			s04_student.std_no = s04_norstusc.std_no and
			//			s04_norstusc.stu_status = 2 --重修
			//		inner join s04_subject on
			//			s04_noropenc.sub_id = s04_subject.sub_id
			//		inner join s04_hstusubjscoreterm on
			//			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//			s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
			//			s04_hstusubjscoreterm.hstusst_status = 2
			//		inner join s04_stddbgo on
			//			s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
			//			s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
			//			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//			s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
			//			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//			s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//		inner join s04_108subject on
			//			s04_stddbgo.course_code = s04_108subject.course_code
			//		inner join s90_organization on
			//			s04_noropenc.org_id = s90_organization.org_id
			//		inner join s90_branch on
			//			s04_noropenc.bra_id = s90_branch.bra_id
			//		inner join s04_hstusixscoreterm on
			//			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//		 join L01_std_attestation a on
			//			a.year_id = s04_stuhcls.year_id and
			//			a.sms_id = s04_stuhcls.sms_id and
			//			a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
			//			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
			//			a.std_no = s04_student.std_no and
			//			isnull(a.attestation_send,'') <>'' and
			//			isnull(a.attestation_status,'') in('Y','F')
			//		left join s04_stubuterm on
			//			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
			//		inner join s04_hstusubjscoreterm hstusst_new on
			//			s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
			//			s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
			//			s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
			//			s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
			//			hstusst_new.hstusst_status = 1
			//		inner join s04_norstusc norstu_o on
			//			hstusst_new.std_no = norstu_o.std_no and
			//			hstusst_new.in_year = norstu_o.in_year and
			//			hstusst_new.in_sms = norstu_o.in_sms and
			//			norstu_o.stu_status = 1
			//		inner join s04_noropenc noro_o on
			//			norstu_o.noroc_id = noro_o.noroc_id and
			//			hstusst_new.sub_id = noro_o.sub_id 		
			//		inner join s90_organization org_o on
			//			noro_o.org_id = org_o.org_id
			//		where s04_student.std_hisdatayear >= 108 
			//	union
			//	SELECT 
			//			as_name = s04_student.std_name,
			//			as_id_no = convert(varchar(10),s04_student.std_identity) ,
			//			as_std_no = convert(varchar(10),s04_student.std_no),
			//			as_grade = convert(smallint,s04_stuhcls.grd_id) ,
			//			as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
			//			as_cls_no = convert(varchar(6),s04_noropenc.noroc_id),
			//			as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
			//			as_cls_group = convert(varchar(20),case when isnull(s04_noropenc.org_id,0) in (0,99) then '無' else s90_organization.org_name end),
			//			as_subj_uid = convert(varchar(10),s04_subject.sub_id),
			//			as_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//			as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
			//			as_credits = convert(varchar(1),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
			//			as_type = convert(varchar(1),s04_norstusc.stu_status),
			//			as_use_credits = convert(varchar(10),case s04_noropenc.is_credit when 'Y' then '1' when 'N' then '0' else '' end ),
			//			as_o_syear = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
			//			as_o_sems = convert(smallint , s04_stddbgo.sms_id ),
			//			as_o_cls_year = convert(varchar(1) , s04_stddbgo.grd_id ),
			//			as_o_cls_no = convert(varchar(1),s04_stddbgo.dep_id) + 
			//				case when s04_stddbgo.bra_id >= 10 then convert(varchar(2),s04_stddbgo.bra_id) else '0' + convert(varchar(1),s04_stddbgo.bra_id) end + 
			//				convert(varchar(1),s04_stddbgo.grd_id) + 
			//				case when s04_stuhcls.cls_id >= 10 then '0' + convert(varchar(2),s04_stuhcls.cls_id) else '00' + convert(varchar(1),s04_stuhcls.cls_id) end,
			//			as_o_cls_name = convert(varchar(60),s04_ytdbgoc.cls_abr),
			//			as_o_cls_group = convert(varchar(20),case when isnull(s04_ytdbgoc.org_id,0) in (0,99) then '無' else org_o.org_name end) ,
			//			as_o_subj_id = convert(varchar(10),s04_stddbgo.sub_id),
			//			as_o_course_id = convert(varchar(23),s04_stddbgo.course_code),
			//			as_o_subj_name = convert(varchar(90),subj108_o.sub108_name),
			//			as_o_credits = convert(smallint , s04_stddbgo.sub_credit) ,
			//			as_makeup_mode = convert(varchar(1),case isnull(s04_hstusubjscoreterm.hstusst_outsch,'0') when '1' then 4 when '2' then 5 when '3' then 6 end),
			//			as_rebuild_mode = null,
			//			as_is_precourse = '0',
			//			as_cur_teaid = convert(varchar(50),s04_noropenc.all_empid),
			//			as_cur_teacode = convert(varchar(50),s04_noropenc.all_empcode),
			//						as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
			//			s04_stuhcls.sch_no as a,
			//			s04_stuhcls.year_id as b,
			//			s04_stuhcls.sms_id as c,
			//			d = convert(varchar(6),s04_noropenc.noroc_id),
			//			e = convert(varchar(10),s04_subject.sub_id),
			//			f = convert(varchar(23),s04_stddbgo.course_code),
			//			g = convert(varchar(50),s04_noropenc.all_empid),
			//			h = s04_student.std_no ,
			//			a.ser_id as i,
			//			s04_noropenc.sub_credit as j,
			//			convert(varchar(1),s04_norstusc.stu_status) as k,
			//			a.attestation_send as l,
			//			a.attestation_date as m,
			//			a.attestation_status as n,
			//			a.is_sys as o,
			//			a.content as p,
			//			convert(varchar(90),s04_108subject.sub108_name) as q,
			//			convert(varchar(50),s04_noropenc.all_empname) as r,
			//			s = 
			//			(
			//			Select SUBSTRING(
			//				(Select ','+ file_name
			//				From L01_std_public_filehub b
			//			where 
			//					b.complex_key = a.sch_no+'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' and
			//					b.class_name = 'StuAttestation' 
			//				For Xml Path(''))
			//			, 2, 8000)
			//			),
			//			u = a.attestation_confirm,
			//			v = a.attestation_release,
			//			w = a.attestation_reason	
			//		FROM s04_student
			//		inner join s04_stuhcls on
			//			s04_student.std_no = s04_stuhcls.std_no and												
			//			s04_stuhcls.sch_no = @sch_no and 
			//			s04_stuhcls.year_id = @year_id and 
			//			s04_stuhcls.sms_id = @sms_id
			//		inner join s90_cha_id on
			//			s04_stuhcls.std_status = s90_cha_id.cha_id and
			//			s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
			//		inner join s04_noropenc on
			//			s04_noropenc.year_id = s04_stuhcls.year_id and
			//			s04_noropenc.sms_id = s04_stuhcls.sms_id  and
			//			charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
			//		inner join s04_norstusc on
			//			s04_norstusc.noroc_id = s04_noropenc.noroc_id and
			//			s04_student.std_no = s04_norstusc.std_no and
			//			s04_norstusc.stu_status = 3 --補修
			//		inner join s04_subject on
			//			s04_noropenc.sub_id = s04_subject.sub_id
			//		inner join s90_organization on
			//			s04_noropenc.org_id = s90_organization.org_id
			//		inner join s04_hstusubjscoreterm on
			//			s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
			//			s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
			//			s04_hstusubjscoreterm.hstusst_status = 3
			//		left join s04_hstusixscoreterm on
			//			s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
			//			s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
			//			s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
			//		left join s04_stubuterm on
			//			s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
			//			s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
			//			s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
			//			s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
			//		inner join s04_stddbgo on
			//			s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
			//			s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
			//			s04_stuhcls.deg_id = s04_stddbgo.deg_id and
			//			s04_stuhcls.dep_id = s04_stddbgo.dep_id and
			//			s04_stuhcls.bra_id = s04_stddbgo.bra_id and
			//			s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
			//			s04_stuhcls.cg_code = s04_stddbgo.cg_code and
			//			isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
			//			s04_noropenc.sub_id = s04_stddbgo.sub_id and
			//			( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
			//		inner join s04_108subject on
			//			s04_stddbgo.course_code = s04_108subject.course_code
			//		inner join s04_ytdbgoc on
			//			s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
			//			s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
			//			s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
			//			s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
			//			s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
			//			s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
			//			s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
			//		inner join s90_organization org_o on
			//			s04_ytdbgoc.org_id = org_o.org_id
			//		inner join s90_branch on
			//			s04_ytdbgoc.bra_id = s90_branch.bra_id
			//		inner join s04_108subject subj108_o on
			//			s04_stddbgo.course_code = subj108_o.course_code
			//		 join L01_std_attestation a on
			//			a.year_id = s04_stuhcls.year_id and
			//			a.sms_id = s04_stuhcls.sms_id and
			//			a.cls_id = convert(varchar(6),s04_noropenc.noroc_id) and
			//			a.sub_id = convert(varchar(10),s04_subject.sub_id) and
			//			a.src_dup = convert(varchar(23),s04_stddbgo.course_code) and
			//			a.emp_id =  convert(varchar(50),s04_noropenc.all_empid) and
			//			a.std_no = s04_student.std_no and
			//			isnull(a.attestation_send,'') <>'' and 
			//			isnull(a.attestation_status,'') in('Y','F')
			//		where s04_student.std_hisdatayear >= 108
			//		) a
			//                           ) AS NewTable
			//                       WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
			//                       order by as_cls_uid,as_course_id";
			#endregion

			string str_sql = @"
												SELECT *,
												x_cnt = 
													(select count(*)
													 from L01_std_public_filehub a
													 where a.class_name = 'StuAttestation'
													 and	a.number_id = NewTable.i
													 and   a.complex_key = NewTable.a +'_'+convert(varchar,NewTable.b)+'_'+convert(varchar,NewTable.c)+'_'+NewTable.d+'_'+NewTable.e+'_'+NewTable.f+'_'+NewTable.g+'_'+NewTable.h+'_0'),
												x_total =
														(select count(*)
														 from L01_std_attestation a
														 where a.sch_no = NewTable.a
														 and a.year_id = NewTable.b
														 and a.sms_id = NewTable.c
														 and a.emp_id = NewTable.g
														 and isnull(a.attestation_send,'') <>'' 
														 and isnull(a.attestation_confirm,'') = 'Y' 
														),
												x_status = ''
												FROM(
												select *,
														ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.d,a.e) AS RowNum
												from (
														select 
														a = L01_std_attestation.sch_no,
														b = L01_std_attestation.year_id,
														c = L01_std_attestation.sms_id ,
														d = L01_std_attestation.cls_id,
														e = L01_std_attestation.sub_id,
														f = L01_std_attestation.src_dup,
														g = L01_std_attestation.emp_id,
														h = L01_std_attestation.std_no,
														i = L01_std_attestation.ser_id,
														j = L01_std_attestation.credit,
														k = L01_std_attestation.stu_status,
														l = L01_std_attestation.attestation_send,
														m = L01_std_attestation.attestation_date,
														n = L01_std_attestation.attestation_status,
														o = L01_std_attestation.is_sys,
														p = L01_std_attestation.content,
														q = L01_std_attestation.sub_name,
														r = L01_std_attestation.all_empname,
														s = 
														(
														Select SUBSTRING(
															(Select ','+ file_name
																From L01_std_public_filehub b
																where 
																b.complex_key = L01_std_attestation.sch_no+'_'+
																				convert(varchar,L01_std_attestation.year_id)+'_'+
																				convert(varchar,L01_std_attestation.sms_id)+'_'+
																				L01_std_attestation.cls_id+'_'+
																				L01_std_attestation.sub_id+'_'+
																				L01_std_attestation.src_dup+'_'+
																				L01_std_attestation.emp_id+'_'+
																				L01_std_attestation.std_no+'_0' and
																b.class_name = 'StuAttestation' 
															For Xml Path(''))
														, 2, 8000)
														),
														u = L01_std_attestation.attestation_confirm,
														v = L01_std_attestation.attestation_release,
														w = L01_std_attestation.attestation_reason,
														as_cls_name = L01_std_attestation.cls_name,
														as_name = std_name,
														as_cur_teaname = L01_std_attestation.all_empname,
														as_subj_uid = L01_std_attestation.sub_id,
														as_cls_uid = L01_std_attestation.cls_id,
														as_std_no = L01_std_attestation.std_no,
														as_subj_name = sub_name
														from L01_std_attestation
														join s04_student on s04_student.std_no = L01_std_attestation.std_no
														join s04_stuhcls on s04_stuhcls.year_id = L01_std_attestation.year_id and
														s04_stuhcls.sms_id = L01_std_attestation.sms_id and
														s04_stuhcls.std_no = L01_std_attestation.std_no and
														s04_stuhcls.grd_id = @grade_id
														where L01_std_attestation.sch_no = @sch_no  
														and L01_std_attestation.year_id = @year_id  
														and L01_std_attestation.sms_id = @sms_id 
														and charindex(@emp_id,convert(varchar(50),emp_id)) > 0
														and isnull(L01_std_attestation.attestation_send,'') <>'' 
														and isnull(L01_std_attestation.attestation_status,'') in('Y','F')
													)a
												) AS NewTable
												WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
												order by d,e";
            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";

            using (IDbConnection conn = _context.CreateCommand())
			{
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
			}
		}

		public async Task<IEnumerable<ClsList>> GetListCls(StuAttestationQueryList arg)
		{
			arg.emp_id = _context.user_id;
			arg.sch_no = _context.SchNo;
			//arg.year_id = _context.now_year;
			//arg.sms_id = _context.now_sms;

            string str_sql = @"

			SELECT 
						cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
							convert(varchar(1),s04_noropenc.dep_id) + 
							case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
							convert(varchar(1),s04_noropenc.grd_id) + 
							case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
						cls_abr = convert(varchar(90),s04_noropenc.noroc_clsname) --開課班級名稱
				FROM s04_student
				inner join s04_stuhcls on
					s04_student.std_no = s04_stuhcls.std_no and												
					s04_stuhcls.sch_no = @sch_no and 
					s04_stuhcls.year_id = @year_id and 
					s04_stuhcls.sms_id = @sms_id 
				inner join s90_cha_id on
					s04_stuhcls.std_status = s90_cha_id.cha_id and
					s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
				inner join s04_norstusc on
					s04_student.std_no = s04_norstusc.std_no and
					s04_norstusc.in_year = s04_stuhcls.year_id and
					s04_norstusc.in_sms = s04_stuhcls.sms_id and
					s04_norstusc.stu_status = 1 --新修
				inner join s04_noropenc on
					s04_norstusc.noroc_id = s04_noropenc.noroc_id and
					isnull(s04_noropenc.course_code,'') <> '' and
					isnull(s04_noropenc.is_learn,'N') = 'Y' and
					charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
				inner join s04_subject on
					s04_noropenc.sub_id = s04_subject.sub_id
				inner join s04_stddbgo on
					s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
					s04_stuhcls.sms_id = s04_stddbgo.sms_id and
					s04_stuhcls.deg_id = s04_stddbgo.deg_id and
					s04_stuhcls.dep_id = s04_stddbgo.dep_id and
					s04_stuhcls.bra_id = s04_stddbgo.bra_id and
					s04_stuhcls.grd_id = s04_stddbgo.grd_id and
					s04_stuhcls.cg_code = s04_stddbgo.cg_code and
					isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
					s04_noropenc.sub_id = s04_stddbgo.sub_id and
					( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
				inner join s04_108subject on
					s04_stddbgo.course_code = s04_108subject.course_code
				left join s90_organization on
					s04_noropenc.org_id = s90_organization.org_id
				left join s90_branch on
					s04_noropenc.bra_id = s90_branch.bra_id
				left join s04_hstusubjscoreterm on
					s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
					s04_hstusubjscoreterm.hstusst_status = 1
				left join s04_hstusixscoreterm on
					s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
				left join s04_stubuterm on
					s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
					s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
					s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
				where s04_student.std_hisdatayear >= 108

			union

			SELECT 
					cls_id = convert(varchar(6),s04_noropenc.noroc_id),
					cls_abr = convert(varchar(90),s04_noropenc.noroc_clsname)
				FROM s04_student
				inner join s04_stuhcls on
					s04_student.std_no = s04_stuhcls.std_no and												
					s04_stuhcls.sch_no = @sch_no and 
					s04_stuhcls.year_id = @year_id and 
					s04_stuhcls.sms_id = @sms_id
				inner join s90_cha_id on
					s04_stuhcls.std_status = s90_cha_id.cha_id and
					s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
				inner join s04_noropenc on
					s04_noropenc.year_id = s04_stuhcls.year_id and
					s04_noropenc.sms_id = s04_stuhcls.sms_id  and
					charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
				inner join s04_norstusc on
					s04_norstusc.noroc_id = s04_noropenc.noroc_id and
					s04_student.std_no = s04_norstusc.std_no and
					s04_norstusc.stu_status = 2 --重修
				inner join s04_subject on
					s04_noropenc.sub_id = s04_subject.sub_id
				inner join s04_hstusubjscoreterm on
					s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
					s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
					s04_hstusubjscoreterm.hstusst_status = 2
				inner join s04_stddbgo on
					s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
					s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
					s04_stuhcls.deg_id = s04_stddbgo.deg_id and
					s04_stuhcls.dep_id = s04_stddbgo.dep_id and
					s04_stuhcls.bra_id = s04_stddbgo.bra_id and
					s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
					s04_stuhcls.cg_code = s04_stddbgo.cg_code and
					isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
					s04_noropenc.sub_id = s04_stddbgo.sub_id and
					( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
				inner join s04_108subject on
					s04_stddbgo.course_code = s04_108subject.course_code
				inner join s90_organization on
					s04_noropenc.org_id = s90_organization.org_id
				inner join s90_branch on
					s04_noropenc.bra_id = s90_branch.bra_id
				inner join s04_hstusixscoreterm on
					s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
				left join s04_stubuterm on
					s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
					s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
					s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
				inner join s04_hstusubjscoreterm hstusst_new on
					s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
					s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
					s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
					s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
					hstusst_new.hstusst_status = 1
				inner join s04_norstusc norstu_o on
					hstusst_new.std_no = norstu_o.std_no and
					hstusst_new.in_year = norstu_o.in_year and
					hstusst_new.in_sms = norstu_o.in_sms and
					norstu_o.stu_status = 1
				inner join s04_noropenc noro_o on
					norstu_o.noroc_id = noro_o.noroc_id and
					hstusst_new.sub_id = noro_o.sub_id 		
				inner join s90_organization org_o on
					noro_o.org_id = org_o.org_id
				where s04_student.std_hisdatayear >= 108 

			union

			SELECT 
					cls_id = convert(varchar(6),s04_noropenc.noroc_id),
					cls_abr = convert(varchar(90),s04_noropenc.noroc_clsname)
				FROM s04_student
				inner join s04_stuhcls on
					s04_student.std_no = s04_stuhcls.std_no and												
					s04_stuhcls.sch_no = @sch_no and 
					s04_stuhcls.year_id = @year_id and 
					s04_stuhcls.sms_id = @sms_id 
				inner join s90_cha_id on
					s04_stuhcls.std_status = s90_cha_id.cha_id and
					s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
				inner join s04_noropenc on
					s04_noropenc.year_id = s04_stuhcls.year_id and
					s04_noropenc.sms_id = s04_stuhcls.sms_id  and
					charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0
				inner join s04_norstusc on
					s04_norstusc.noroc_id = s04_noropenc.noroc_id and
					s04_student.std_no = s04_norstusc.std_no and
					s04_norstusc.stu_status = 3 --補修
				inner join s04_subject on
					s04_noropenc.sub_id = s04_subject.sub_id
				inner join s90_organization on
					s04_noropenc.org_id = s90_organization.org_id
				inner join s04_hstusubjscoreterm on
					s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
					s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
					s04_hstusubjscoreterm.hstusst_status = 3
				left join s04_hstusixscoreterm on
					s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
					s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
					s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
				left join s04_stubuterm on
					s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
					s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
					s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
					s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
				inner join s04_stddbgo on
					s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
					s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
					s04_stuhcls.deg_id = s04_stddbgo.deg_id and
					s04_stuhcls.dep_id = s04_stddbgo.dep_id and
					s04_stuhcls.bra_id = s04_stddbgo.bra_id and
					s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
					s04_stuhcls.cg_code = s04_stddbgo.cg_code and
					isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
					s04_noropenc.sub_id = s04_stddbgo.sub_id and
					( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
				inner join s04_108subject on
					s04_stddbgo.course_code = s04_108subject.course_code
				inner join s04_ytdbgoc on
					s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
					s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
					s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
					s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
					s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
					s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
					s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
				inner join s90_organization org_o on
					s04_ytdbgoc.org_id = org_o.org_id
				inner join s90_branch on
					s04_ytdbgoc.bra_id = s90_branch.bra_id
				inner join s04_108subject subj108_o on
					s04_stddbgo.course_code = subj108_o.course_code
				where s04_student.std_hisdatayear >= 108
			       order by cls_id";
            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
			{
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
                arg.emp_id = getemp.ElementAt(0).emp_id;

                return await conn.QueryAsync<ClsList>(str_sql, arg);
			}
		}

		public async Task<IEnumerable<HeaderList>> GetListStuStatus(StuAttestationQueryList arg)
		{
			arg.sch_no = _context.SchNo;
			string str_sql = @"
								SELECT *,
									x_total =
									(select count(*)
										from s04_norstusc a,s04_noropenc b,s04_student c
										where	convert(varchar,NewTable.b)= a.in_year and 
													convert(varchar,NewTable.c) = a.in_sms and 
													NewTable.as_cur_teaname = convert(varchar(50),b.all_empname) and 
													c.std_no = a.std_no and
													a.noroc_id = b.noroc_id and
													a.stu_status in(1) and
													isnull(b.course_code,'') <> '' and
													isnull(b.is_learn,'N') = 'Y' and
													(
														((@std_no = '' or @std_no is null) or (@std_no <> '' and a.std_no =  NewTable.as_std_no))and
														((@std_name = '' or @std_name is null) or (@std_name <> '' and c.std_name = NewTable.as_name)
														)
													)and
												(
													(@cls_id = '' or @cls_id is null) or
													(@cls_id <> '' and
													convert(varchar(6),case b.noroc_bclass when 'Y' then  --開課班級內碼
													convert(varchar(1),b.dep_id) + 
													case when b.bra_id >= 10 then convert(varchar(2),b.bra_id) else '0' + convert(varchar(1),b.bra_id) end + 
													convert(varchar(1),b.grd_id) + 
													case when b.cls_id >= 10 then convert(varchar(2),b.cls_id) else '0' + convert(varchar(1),b.cls_id) end else convert(varchar(6) , b.noroc_id ) end )=NewTable. as_cls_uid)
												)
									)										
									+
									(select count(*)
										from s04_norstusc a,s04_noropenc b,s04_student c
										where convert(varchar,NewTable.b)= b.year_id and 
													convert(varchar,NewTable.c) = b.sms_id and 
													c.std_no = a.std_no and
													NewTable.as_cur_teaname = convert(varchar(50),b.all_empname) and 
													a.noroc_id = b.noroc_id and
													a.stu_status in(2,3)  and
													(
														((@std_no = '' or @std_no is null) or (@std_no <> '' and a.std_no =  NewTable.as_std_no))and
														((@std_name = '' or @std_name is null) or (@std_name <> '' and c.std_name = NewTable.as_name))
													) and
												(
													(@cls_id = '' or @cls_id is null) or (@cls_id <> '' and convert(varchar(6),b.noroc_id) = NewTable. as_cls_uid)
												)
									)
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY a.b,a.c,a.as_cls_uid,a.as_subj_uid) AS RowNum
										from (
										SELECT 
												as_sit_num = s04_stuhcls.sit_num,
												as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
												as_subj_uid = convert(varchar(10),s04_subject.sub_id), --科目內碼
												as_cls_uid = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
													convert(varchar(1),s04_noropenc.dep_id) + 
													case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
													convert(varchar(1),s04_noropenc.grd_id) + 
													case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												as_name = s04_student.std_name,
												as_std_no = convert(varchar(10),s04_student.std_no),			--學號
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname), --開課班級名稱
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name), --科目名稱
												x_fileupload = isnull((
																select count(*)
																from L01_std_attestation a
																join L01_std_public_filehub b on 
																									a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																									b.class_name = 'StuAttestation' and
																								    a.ser_id = b.number_id
																where a.year_id =  s04_stuhcls.year_id
																and   a.sms_id = s04_stuhcls.sms_id
																and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																					convert(varchar(1),s04_noropenc.dep_id) + 
																					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																					convert(varchar(1),s04_noropenc.grd_id) + 
																					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																and   a.std_no = s04_student.std_no),0),
												x_pass = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	and   a.attestation_date <> ''
																	and   a.attestation_status = 'Y'
																	and   a.attestation_release = 'Y'),0),
												x_notyet = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	--and   a.attestation_date = ''
																	--and   a.attestation_status = 'N'
																	and   (a.attestation_release = '' or a.attestation_release  is null)),0)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id  and
												((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no = @std_no))and
												((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name))
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
											inner join s04_norstusc on
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.in_year = s04_stuhcls.year_id and
												s04_norstusc.in_sms = s04_stuhcls.sms_id and
												s04_norstusc.stu_status = 1 --新修
											inner join s04_noropenc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												isnull(s04_noropenc.course_code,'') <> '' and
												isnull(s04_noropenc.is_learn,'N') = 'Y'and
												charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0  and
												(
													(@cls_id = '' or @cls_id is null) or
													(@cls_id <> '' and
													convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
													convert(varchar(1),s04_noropenc.dep_id) + 
													case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
													convert(varchar(1),s04_noropenc.grd_id) + 
													case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )= @cls_id)
												)
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s04_stddbgo on
												s04_stuhcls.year_id - s04_stuhcls.grd_id + 1 = s04_stddbgo.year_id and
												s04_stuhcls.sms_id = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_stuhcls.grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											left join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											left join s90_branch on
												s04_noropenc.bra_id = s90_branch.bra_id
											left join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_hstusubjscoreterm.hstusst_status = 1
											left join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
											where s04_student.std_hisdatayear >= 108

										union

										SELECT 
												as_sit_num = s04_stuhcls.sit_num,
												as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
												as_subj_uid = convert(varchar(10),s04_subject.sub_id),
												as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												as_name = s04_student.std_name,
												as_std_no = convert(varchar(10),s04_student.std_no),			--學號
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
												x_fileupload = isnull((
																select count(*)
																from L01_std_attestation a
																join L01_std_public_filehub b on 
																									a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																									b.class_name = 'StuAttestation'  and
																								    a.ser_id = b.number_id
																where a.year_id =  s04_stuhcls.year_id
																and   a.sms_id = s04_stuhcls.sms_id
																and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																					convert(varchar(1),s04_noropenc.dep_id) + 
																					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																					convert(varchar(1),s04_noropenc.grd_id) + 
																					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																and   a.std_no = s04_student.std_no),0),
												x_pass = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	and   a.attestation_date <> ''
																	and   a.attestation_status = 'Y'
																	and   a.attestation_release = 'Y'),0),
												x_notyet = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	--and   a.attestation_date = ''
																	--and   a.attestation_status = 'N'
																	and   (a.attestation_release = '' or a.attestation_release  is null)),0)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id  and
												((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no = @std_no))and
												((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name))
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370') --只上傳正常學籍、休學、輔導延修
											inner join s04_noropenc on
												s04_noropenc.year_id = s04_stuhcls.year_id and
												s04_noropenc.sms_id = s04_stuhcls.sms_id and
												charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0  and
												((@cls_id = '' or @cls_id is null) or (@cls_id <> '' and convert(varchar(6),s04_noropenc.noroc_id) = @cls_id))
											inner join s04_norstusc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.stu_status = 2 --重修
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
												s04_hstusubjscoreterm.hstusst_status = 2
											inner join s04_stddbgo on
												s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
												s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											inner join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											inner join s90_branch on
												s04_noropenc.bra_id = s90_branch.bra_id
											inner join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id 
											inner join s04_hstusubjscoreterm hstusst_new on
												s04_hstusubjscoreterm.sub_id = hstusst_new.sub_id and
												s04_hstusubjscoreterm.std_no = hstusst_new.std_no and
												s04_hstusubjscoreterm.in_year = hstusst_new.in_year and
												s04_hstusubjscoreterm.in_sms = hstusst_new.in_sms and
												hstusst_new.hstusst_status = 1
											inner join s04_norstusc norstu_o on
												hstusst_new.std_no = norstu_o.std_no and
												hstusst_new.in_year = norstu_o.in_year and
												hstusst_new.in_sms = norstu_o.in_sms and
												norstu_o.stu_status = 1
											inner join s04_noropenc noro_o on
												norstu_o.noroc_id = noro_o.noroc_id and
												hstusst_new.sub_id = noro_o.sub_id 		
											inner join s90_organization org_o on
												noro_o.org_id = org_o.org_id
											where s04_student.std_hisdatayear >= 108 

										union

										SELECT 
												as_sit_num = s04_stuhcls.sit_num,
												as_cur_teaname = convert(varchar(50),s04_noropenc.all_empname),
												as_subj_uid = convert(varchar(10),s04_subject.sub_id),
												as_cls_uid = convert(varchar(6),s04_noropenc.noroc_id),
												s04_stuhcls.sch_no as a,
												s04_stuhcls.year_id as b,
												s04_stuhcls.sms_id as c,
												as_name = s04_student.std_name,
												as_std_no = convert(varchar(10),s04_student.std_no),			--學號
												as_cls_name = convert(varchar(90),s04_noropenc.noroc_clsname),
												as_subj_name = convert(varchar(90),s04_108subject.sub108_name),
												x_fileupload = isnull((
																select count(*)
																from L01_std_attestation a
																join L01_std_public_filehub b on 
																									a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																									b.class_name = 'StuAttestation' and
																								    a.ser_id = b.number_id
																where a.year_id =  s04_stuhcls.year_id
																and   a.sms_id = s04_stuhcls.sms_id
																and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																					convert(varchar(1),s04_noropenc.dep_id) + 
																					case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																					convert(varchar(1),s04_noropenc.grd_id) + 
																					case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																and   a.std_no = s04_student.std_no),0),
												x_pass = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	and   a.attestation_date <> ''
																	and   a.attestation_status = 'Y'
																	and   a.attestation_release = 'Y'),0),
												x_notyet = isnull((
																	select count(*)
																	from L01_std_attestation a
																	join L01_std_public_filehub b on 
																										a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																										b.class_name = 'StuAttestation'  and
																									    a.ser_id = b.number_id
																	where  a.year_id =  s04_stuhcls.year_id
																	and   a.sms_id = s04_stuhcls.sms_id
																	and   a.cls_id = convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then 
																						convert(varchar(1),s04_noropenc.dep_id) + 
																						case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
																						convert(varchar(1),s04_noropenc.grd_id) + 
																						case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end )
																	and   a.sub_id = convert(varchar(10),s04_subject.sub_id)
																	and   a.src_dup = convert(varchar(23),s04_stddbgo.course_code)
																	and   a.emp_id =  convert(varchar(50),s04_noropenc.all_empid)
																	and   a.std_no = s04_student.std_no
																	and   a.attestation_send <> ''
																	--and   a.attestation_date = ''
																	--and   a.attestation_status = 'N'
																	and   (a.attestation_release = '' or a.attestation_release  is null)),0)	
											FROM s04_student
											inner join s04_stuhcls on
												s04_student.std_no = s04_stuhcls.std_no and												
												s04_stuhcls.sch_no = @sch_no and 
												s04_stuhcls.year_id = @year_id and 
												s04_stuhcls.sms_id = @sms_id  and
												((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no = @std_no))and
												((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name))
											inner join s90_cha_id on
												s04_stuhcls.std_status = s90_cha_id.cha_id and
												s90_cha_id.cha_code in ('A00','341','342','343','344','345','346','347','348','349','350','364','368','370')
											inner join s04_noropenc on
												s04_noropenc.year_id = s04_stuhcls.year_id and
												s04_noropenc.sms_id = s04_stuhcls.sms_id  and
												charindex(@emp_id,convert(varchar(50),s04_noropenc.all_empid)) > 0 and
												((@cls_id = '' or @cls_id is null) or (@cls_id <> '' and convert(varchar(6),s04_noropenc.noroc_id) = @cls_id))
											inner join s04_norstusc on
												s04_norstusc.noroc_id = s04_noropenc.noroc_id and
												s04_student.std_no = s04_norstusc.std_no and
												s04_norstusc.stu_status = 3 --補修
											inner join s04_subject on
												s04_noropenc.sub_id = s04_subject.sub_id
											inner join s90_organization on
												s04_noropenc.org_id = s90_organization.org_id
											inner join s04_hstusubjscoreterm on
												s04_stuhcls.std_no = s04_hstusubjscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusubjscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_noropenc.sub_id = s04_hstusubjscoreterm.sub_id and
												s04_noropenc.sub_credit = s04_hstusubjscoreterm.sub_credit and
												s04_hstusubjscoreterm.hstusst_status = 3
											left join s04_hstusixscoreterm on
												s04_stuhcls.std_no = s04_hstusixscoreterm.std_no and
												s04_stuhcls.year_id = s04_hstusixscoreterm.year_id and
												s04_stuhcls.sms_id = s04_hstusixscoreterm.sms_id 
											left join s04_stubuterm on
												s04_stubuterm.std_no = s04_hstusubjscoreterm.std_no and
												s04_stubuterm.year_id = s04_hstusubjscoreterm.year_id and
												s04_stubuterm.sms_id = s04_hstusubjscoreterm.sms_id and
												s04_stubuterm.sub_id = s04_hstusubjscoreterm.sub_id
											inner join s04_stddbgo on
												s04_hstusubjscoreterm.in_year - s04_hstusubjscoreterm.in_grd_id + 1 = s04_stddbgo.year_id and
												s04_hstusubjscoreterm.in_sms = s04_stddbgo.sms_id and
												s04_stuhcls.deg_id = s04_stddbgo.deg_id and
												s04_stuhcls.dep_id = s04_stddbgo.dep_id and
												s04_stuhcls.bra_id = s04_stddbgo.bra_id and
												s04_hstusubjscoreterm.in_grd_id = s04_stddbgo.grd_id and
												s04_stuhcls.cg_code = s04_stddbgo.cg_code and
												isnull(s04_stuhcls.stuc_id,0) = s04_stddbgo.stuc_id and
												s04_noropenc.sub_id = s04_stddbgo.sub_id and
												( s04_stddbgo.course_cate not in ('8','9') or ( s04_stddbgo.course_cate in ('8','9') and s04_stddbgo.course_type = 'D' ) )
											inner join s04_108subject on
												s04_stddbgo.course_code = s04_108subject.course_code
											inner join s04_ytdbgoc on
												s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 = s04_ytdbgoc.year_id and
												s04_stddbgo.sms_id = s04_ytdbgoc.sms_id and
												s04_stddbgo.dep_id = s04_ytdbgoc.dep_id and
												s04_stddbgo.deg_id = s04_ytdbgoc.deg_id and
												s04_stddbgo.bra_id = s04_ytdbgoc.bra_id and
												s04_stddbgo.grd_id = s04_ytdbgoc.grd_id and
												s04_stuhcls.cls_id = s04_ytdbgoc.cls_id
											inner join s90_organization org_o on
												s04_ytdbgoc.org_id = org_o.org_id
											inner join s90_branch on
												s04_ytdbgoc.bra_id = s90_branch.bra_id
											inner join s04_108subject subj108_o on
												s04_stddbgo.course_code = subj108_o.course_code
											where s04_student.std_hisdatayear >= 108
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by as_cls_uid,as_subj_uid,as_std_no";

			string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
			using (IDbConnection conn = _context.CreateCommand())
			{
                IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);
				arg.emp_id = getemp.ElementAt(0).emp_id;       
                return await conn.QueryAsync<HeaderList>(str_sql, arg);
			}
		}

        public async Task<IEnumerable<HeaderList>> GetListTutor(StuAttestationQueryList arg)
        {
			string str_teaconsult = string.Empty;

            if (arg.kind == "admin") 
			{
				arg.emp_id = "";
			}
			else if(arg.kind == "teaconsult")
			{
				str_teaconsult = @"
													join L03_tea_consult_setup on 
														s04_ytdbgoc.year_id = L03_tea_consult_setup.year_id and
														s04_ytdbgoc.sms_id = L03_tea_consult_setup.sms_id and
														s04_ytdbgoc.deg_id = L03_tea_consult_setup.deg_id and
														s04_ytdbgoc.dep_id = L03_tea_consult_setup.dep_id and
														s04_ytdbgoc.bra_id = L03_tea_consult_setup.bra_id and
														s04_ytdbgoc.grd_id = L03_tea_consult_setup.grd_id and
														s04_ytdbgoc.cls_id = L03_tea_consult_setup.cls_id and
														L03_tea_consult_setup.emp_id = @consult_emp														
					";
                    arg.emp_id = "";
            };

            arg.sch_no = _context.SchNo;
            string str_sql =string.Format(@"
								select 
									b = s04_stuhcls.year_id,
									c = s04_stuhcls.sms_id,
									as_cls_name = s04_ytdbgoc.cls_abr,
									as_std_no = s04_student.std_no,
									as_name = s04_student.std_name,
									x_1 = isnull((
														select count(*)
														from L01_std_attestation a
														join L01_std_public_filehub b on 
																							a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																							b.class_name = 'StuAttestation'
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   a.attestation_send <> ''
														and   a.attestation_date <> ''
														and   a.attestation_status = 'Y'),0),
									x_2 = isnull((
														select count(*)
														from L01_std_attestation a
														join L01_std_public_filehub b on 
																							a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																							b.class_name = 'StuAttestation'
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   a.attestation_send <> ''
														and   a.attestation_date <> ''
														and   a.attestation_status = 'Y'
														and   a.attestation_release = 'Y'),0),
									x_3 = isnull((
														select count(*)
														from L01_std_attestation a
														join L01_std_public_filehub b on 
																							a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																							b.class_name = 'StuAttestation'
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   a.attestation_send <> ''
														and   a.attestation_date <> ''
														and   a.attestation_status = 'Y'
														and   a.attestation_release = 'Y'
														and   a.attestation_centraldb = 'Y'),0),
									x_4 = isnull((
														select count(*)
														from L01_std_position a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_college a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_competition a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_group a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_license a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_other a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_result a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_study_free a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_volunteer a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0)+
										isnull((
														select count(*)
														from L01_stu_workplace a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no),0),
									x_5 = isnull((
														select count(*)
														from L01_std_position a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_college a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_competition a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_group a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_license a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_other a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_result a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_study_free a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_volunteer a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)+
										isnull((
														select count(*)
														from L01_stu_workplace a
														where  a.year_id =  s04_stuhcls.year_id
														and   a.sms_id = s04_stuhcls.sms_id
														and   a.std_no = s04_student.std_no
														and   isnull(a.check_centraldb,'') = 'Y'),0)
								from s04_student
								inner join s04_stuhcls on 
									s04_stuhcls.std_no = s04_student.std_no
								inner join s04_ytdbgoc on 
									s04_ytdbgoc.year_id = s04_stuhcls.year_id and
									s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
									s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
									s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
									s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
									s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
								inner join s90_employee on s90_employee.emp_id = s04_ytdbgoc.emp_id
								{0}
								where 
									s04_stuhcls.year_id = @year_id and
									s04_stuhcls.sms_id = @sms_id and 
									((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no = @std_no)) and
									((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name)) and
									((@emp_id = '' or @emp_id is null) or (@emp_id <> '' and s90_employee.emp_code = @emp_id))
								order by s04_ytdbgoc.cls_abr,s04_student.std_no", str_teaconsult);

            string str_sql_consult_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @consult_emp
														";
            string str_sql_emp = @"
														select emp_id
														from s90_employee
														where emp_code = @emp_id
														";
            using (IDbConnection conn = _context.CreateCommand())
            {
				if (arg.kind == "admin")
				{
				}
				else if (arg.kind == "teaconsult")
				{
					IEnumerable<GetEmpId> getconsultemp = conn.Query<GetEmpId>(str_sql_consult_emp, arg);

					if (getconsultemp.Count() > 0)
					{
						arg.consult_emp = getconsultemp.ElementAt(0).emp_id;
					}
					else 
					{
						arg.consult_emp = "";

                    }                   
				}
				else {
                    IEnumerable<GetEmpId> getemp = conn.Query<GetEmpId>(str_sql_emp, arg);

					if (getemp.Count() > 0)
					{
                        arg.emp_id = getemp.ElementAt(0).emp_id;
                    }
					else 
					{
						arg.emp_id = "";
                    }                    
                };

                return await conn.QueryAsync<HeaderList>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<L01_StuAttestation>> GetFormData(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
                                SELECT *
                                FROM L01_stu_group
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and std_no = @std_no
                                and ser_id = @ser_id
                            ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<L01_StuAttestation>(str_sql, arg);
            }
        }

        public async Task<int> UpdateData(L01_StuAttestation arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;

            string str_sql = @"
                                update L01_std_attestation
                                set 
                                    attestation_send =  @upd_dt,
                                    attestation_status = 'N'
                                WHERE sch_no = @sch_no 
                                and year_id = @year_id 
                                and sms_id = @sms_id 
                                and cls_id = @cls_id
                                and sub_id = @sub_id
                                and src_dup = @src_dup
                                and emp_id = @emp_id
                                and std_no = @std_no
								and ser_id = @ser_id
                            ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<int> UpdateDataConfirm(L01_StuAttestationList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            int k = 1;
            foreach (var item in arg.complex_key)
            {
                sb.Append(string.Format(@"
                                update L01_std_attestation
                                set 
                                    attestation_status =  @arg2_{0},
									attestation_date = @arg3_{2}
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no +'_'+convert(varchar,ser_id)= @arg1_{1}
                            ", i, j, k));

                dynamicParams.Add("arg1_" + i, item.Split('@')[0]);
                dynamicParams.Add("arg2_" + j, item.Split('@')[1]);
                dynamicParams.Add("arg3_" + k, updteDate());
                i++;
                j++;
                k++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }

        public async Task<int> UpdateDataConfirmRelease(L01_StuAttestationList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var item in arg.complex_key)
            {
                sb.Append(string.Format(@"
                                update L01_std_attestation
                                set 
                                    attestation_release =  'Y'
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_'+convert(varchar,ser_id) = @arg1_{0}
                            ", i));

                dynamicParams.Add("arg1_" + i, item.Split('@')[0]);
                i++;
            };

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
            }
        }

        public async Task<int> UpdateDataConfirmReason(L01_StuAttestationList arg)
        {
            //arg.sch_no = _schno;
            //arg.upd_dt = updteDate();
            //arg.upd_name = arg.std_no;

            var dynamicParams = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            int j = 1;
            foreach (var item in arg.complex_key)
            {
                sb.Append(string.Format(@"
                                update L01_std_attestation
                                set 
                                    attestation_reason =  @arg2_{0}
                                WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_'+convert(varchar,ser_id)  = @arg1_{1}
                            ", i, j));

                dynamicParams.Add("arg1_" + i, item.Split('@')[0]);
                dynamicParams.Add("arg2_" + j, item.Split('@')[1]);
                i++;
                j++;
            };
            //string str_sql = @"
            //                    update L01_std_attestation
            //                    set 
            //                        attestation_centraldb =  'Y'
            //                    WHERE sch_no +'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no in @complex_key
            //                ";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.ExecuteAsync(sb.ToString(), dynamicParams);
                //return await conn.ExecuteAsync(str_sql, arg);
            }
        }

        public async Task<Dictionary<string, string>> InsertData(L01_StuAttestation arg)
        {
            arg.sch_no = _context.SchNo;
            arg.upd_dt = updteDate();
            arg.upd_name = arg.std_no;
            string str_sql = "";
            var dict = new Dictionary<string, string>();
            str_sql = @"select * 
                        from L01_stu_group 
                        where year_id = @year_id
                        and sms_id = @sms_id
                        and std_no = @std_no
                        and sch_no = @sch_no
                        ";
            using (IDbConnection conn = _context.CreateCommand())
            {
                var dr = await conn.ExecuteReaderAsync(str_sql, arg);
                DataTable dt = new DataTable();
                dt.Load(dr);
                /*
                var result = (from r in dt.AsEnumerable()
                              where
                                   r.Field<string>("time_id") == arg.time_id &&
                                    r.Field<string>("startdate") == arg.startdate &&
                                    r.Field<string>("enddate") == arg.enddate
                              select r).Count();
                */
                var result = 0;
                if (result > 0)
                {
                    dict.Add("haveData", "Y");
                    dict.Add("result", "0");
                    return dict;
                }
                else
                {
                    var num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Count();

                    if (num > 0)
                    {
                        num = (from r in dt.AsEnumerable()
                               where r.Field<Int16>("year_id") == arg.year_id &&
                                     r.Field<Int16>("sms_id") == arg.sms_id &&
                                     r.Field<string>("std_no") == arg.std_no &&
                                     r.Field<string>("sch_no") == arg.sch_no
                               select r).Max(x => x.Field<Int16>("ser_id")) + 1;
                    }
                    else 
                    {
                        num = 1;
                    }
                    arg.ser_id = num;
                    str_sql = @"
                            insert L01_stu_group
                            (sch_no,year_id,sms_id,ser_id,std_no,time_id,unit_name,group_content,startdate,
                            enddate,hours,content,is_sys,upd_name,upd_dt)
                            values
                            (@sch_no,@year_id,@sms_id,@ser_id,@std_no,@time_id,@unit_name,@group_content,@startdate,
                            @enddate,@hours,@content,@is_sys,@upd_name,@upd_dt)
                        ";
                    var result_count = await conn.ExecuteAsync(str_sql, arg);
                    dict.Add("haveData", "Y");
                    dict.Add("result", result_count.ToString());
                    
                    return dict;
                }
            }
        }

        public async Task<IEnumerable<LearningResult>> GetListLearningResult(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
								SELECT *,
									x_total =
														(
														select  count(*)
														from s04_student
														inner join s04_stuhcls on 
															s04_stuhcls.std_no = s04_student.std_no
														inner join s04_ytdbgoc on 
															s04_ytdbgoc.year_id = s04_stuhcls.year_id and
															s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
															s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
															s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
															s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
															s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
														where 
															s04_stuhcls.year_id = @year_id and
															s04_stuhcls.sms_id = @sms_id and
															(
																((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no =  NewTable.std_no))and
																((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = NewTable.std_name))
															)
														)
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY cls_abr,std_no) AS RowNum
										from (
													select 
														s04_ytdbgoc.year_id,
														s04_ytdbgoc.sms_id,
														s04_ytdbgoc.cls_abr,
														s04_student.std_no,
														s04_student.std_name,
														x_cnt = 
																		isnull(
																		(select  count(*)
																		from L01_std_attestation a
																		where a.year_id = s04_ytdbgoc.year_id
																		and    a.sms_id = s04_ytdbgoc.sms_id
																		and   a.std_no = s04_student.std_no
																		and   a.attestation_send <> ''
																		and   a.attestation_date <> ''
																		and   a.attestation_status = 'Y'
																		and   a.attestation_release = 'Y'
																		and  isnull(a.attestation_centraldb,'') = 'Y'),0)
													from s04_student
													inner join s04_stuhcls on 
														s04_stuhcls.std_no = s04_student.std_no
													inner join s04_ytdbgoc on 
														s04_ytdbgoc.year_id = s04_stuhcls.year_id and
														s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
														s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
														s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
														s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
														s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
													where 
														s04_stuhcls.year_id = @year_id and
														s04_stuhcls.sms_id = @sms_id and
														(
															((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no =  @std_no))and
															((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name))
														)
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by cls_abr,std_no";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<LearningResult>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<MultipleLearning>> GetListMultipleLearning(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
								SELECT *,
									x_total =
														(
														select  count(*)
														from s04_student
														inner join s04_stuhcls on 
															s04_stuhcls.std_no = s04_student.std_no
														inner join s04_ytdbgoc on 
															s04_ytdbgoc.year_id = s04_stuhcls.year_id and
															s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
															s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
															s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
															s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
															s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
														where 
															s04_stuhcls.year_id = @year_id and
															s04_stuhcls.sms_id = @sms_id and
															(
																((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no =  NewTable.std_no))and
																((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = NewTable.std_name))
															)
														)
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY cls_abr,std_no) AS RowNum
										from (
													select 
														s04_ytdbgoc.year_id,
														s04_ytdbgoc.sms_id,
														s04_ytdbgoc.cls_abr,
														s04_student.std_no,
														s04_student.std_name,
														x_cnt = 
																			isnull((
																							select count(*)
																							from L01_std_position a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_college a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_competition a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_group a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_license a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_other a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_result a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_study_free a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_volunteer a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)+
																			isnull((
																							select count(*)
																							from L01_stu_workplace a
																							where  a.year_id =  s04_stuhcls.year_id
																							and   a.sms_id = s04_stuhcls.sms_id
																							and   a.std_no = s04_student.std_no
																							and   isnull(a.check_centraldb,'') = 'Y'),0)
													from s04_student
													inner join s04_stuhcls on 
														s04_stuhcls.std_no = s04_student.std_no
													inner join s04_ytdbgoc on 
														s04_ytdbgoc.year_id = s04_stuhcls.year_id and
														s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
														s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
														s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
														s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
														s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
													where 
														s04_stuhcls.year_id = @year_id and
														s04_stuhcls.sms_id = @sms_id and
														(
															((@std_no = '' or @std_no is null) or (@std_no <> '' and s04_student.std_no =  @std_no))and
															((@std_name = '' or @std_name is null) or (@std_name <> '' and s04_student.std_name = @std_name))
														)
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by cls_abr,std_no";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<MultipleLearning>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<AttestationNotYet>> GetListAttestationNotYet(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
								SELECT *,
									x_total =
														(
														select  count(distinct emp_id)
														from L01_std_attestation
														where 
															L01_std_attestation.year_id = @year_id and
															L01_std_attestation.sms_id = @sms_id and
														   L01_std_attestation.attestation_send <> '' and
													      (L01_std_attestation.attestation_release = '' or L01_std_attestation.attestation_release  is null)
														)
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY  emp_id) AS RowNum
										from (
													select 
																a.year_id,
																a.sms_id,
																s90_employee.emp_name,
																s90_employee.emp_id,
																x_cnt = count(*)
													from L01_std_attestation a
													join L01_std_public_filehub b on 
															 a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
															 a.ser_id = b.number_id and 
															 b.class_name = 'StuAttestation'
													join s90_employee  on 
														  a.emp_id = s90_employee.emp_id
													where  a.year_id = @year_id
													and   a.sms_id = @sms_id
													and   a.attestation_send <> ''
													and   (a.attestation_release = '' or a.attestation_release  is null)
													group by
														a.year_id,
														a.sms_id,
														s90_employee.emp_name,
														s90_employee.emp_id
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by emp_id";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<AttestationNotYet>(str_sql, arg);
            }
        }

        public async Task<IEnumerable<AttestationNotYet>> GetListAttestationStdNotYet(StuAttestationQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string str_sql = @"
								SELECT *,
									x_total =
														(
														select  count(distinct std_no)
														from L01_std_attestation
														where 
															L01_std_attestation.year_id = NewTable.year_id and
															L01_std_attestation.sms_id = NewTable.sms_id and
															L01_std_attestation.emp_id = NewTable.emp_id and
														   L01_std_attestation.attestation_send <> '' and
													      (L01_std_attestation.attestation_release = '' or L01_std_attestation.attestation_release  is null)
														)
                                FROM(
										select *,
												ROW_NUMBER() OVER(ORDER BY  cls_abr,std_no,sub_name) AS RowNum
										from (
														select 
																	a.year_id,
																	a.sms_id,
																	s04_ytdbgoc.cls_abr,
																	sub_name = convert(varchar(90),s04_108subject.sub108_name),
																	s04_student.std_no,
																	s04_student.std_name,
																	a.emp_id,
																	x_cnt = count(*)
															from L01_std_attestation a
															join L01_std_public_filehub b on 
																		a.sch_no+'_'+convert(varchar,a.year_id)+'_'+CONVERT(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' = b.complex_key and
																		a.ser_id = b.number_id and 
																		b.class_name = 'StuAttestation'
															join s04_student on a.std_no = s04_student.std_no
															join s04_stuhcls  on 
																s04_stuhcls.year_id = a.year_id and
																s04_stuhcls.sms_id = a.sms_id and
																s04_stuhcls.std_no = s04_student.std_no
															inner join s04_ytdbgoc on 
																s04_ytdbgoc.year_id = s04_stuhcls.year_id and
																s04_ytdbgoc.sms_id = s04_stuhcls.sms_id and
																s04_ytdbgoc.dep_id = s04_stuhcls.dep_id and
																s04_ytdbgoc.bra_id = s04_stuhcls.bra_id and
																s04_ytdbgoc.grd_id = s04_stuhcls.grd_id and
																s04_ytdbgoc.cls_id = s04_stuhcls.cls_id 
														inner join s04_108subject on
															a.src_dup = s04_108subject.course_code
														where  a.year_id = @year_id
															and   a.sms_id = @sms_id
															and   a.emp_id = @emp_id
															and   a.attestation_send <> ''
															and   (a.attestation_release = '' or a.attestation_release  is null)
															group by
																	a.year_id,
																	a.sms_id,
																	a.emp_id,
																	s04_ytdbgoc.cls_abr,
																	convert(varchar(90),s04_108subject.sub108_name),
																	s04_student.std_no,
																	s04_student.std_name
											) a
                                    ) AS NewTable
                                WHERE RowNum >= @sRowNun AND RowNum <= @eRowNun
                                order by  cls_abr,std_no,sub_name";

            using (IDbConnection conn = _context.CreateCommand())
            {
                return await conn.QueryAsync<AttestationNotYet>(str_sql, arg);
            }
        }
    }
}
