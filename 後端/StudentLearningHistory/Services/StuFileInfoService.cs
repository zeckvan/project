using Dapper;
using StudentLearningHistory.Context;
using StudentLearningHistory.Models.StuFileInfo.Parameters;
using System.Data;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Reflection;
using StudentLearningHistory.Models.StuPublickFileHub.DB;
using StudentLearningHistory.Models.StudCadre.DTOs;
using Microsoft.IdentityModel.Tokens;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;

namespace StudentLearningHistory.Services
{
    public class StuFileInfoService
    {
        private readonly IDapperContext _context;
        private readonly IConfiguration _configuration;
        public string _schno;
        private string updteDate() => $"{DateTime.Now.Year - 1911}{DateTime.Now.ToString("MMddHHmmss")}";

        public StuFileInfoService(IDapperContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _schno = _context.SchNo;
        }

        public async Task<IEnumerable<L01_std_public_filehub_DTO>> GetFileList(StuFileInfoQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string sqlfile = $@"
                                select *
                                FROM (
                                    SELECT 
                                        ROW_NUMBER() OVER (ORDER BY number_id) AS RowNum,
                                        complex_key,
                                        number_id ,file_name ,file_extension
                                        ,CASE
                                            WHEN file_extension = 'mp3' or file_extension = 'mp4' THEN 2
                                            ELSE 1
                                        END AS file_class,
										case when isnull(upd_dt,'') = '' then '' 
										else substring(upd_dt,1,3)+'/'+
										substring(upd_dt,4,2)+'/'+
										substring(upd_dt,6,2)+' '+
										substring(upd_dt,8,2)+':'+
										substring(upd_dt,10,2)+':'+
										substring(upd_dt,12,2)
										end  as upd_dt,
                                         attestation_file_yn = 
                                                                                isnull(
                                                                                (select  a.check_yn
                                                                                from L01_std_attestation_file a
                                                                                where a.complex_key = L01_std_public_filehub.complex_key
                                                                                and a.class_name = L01_std_public_filehub.class_name
                                                                                and a.type_id = L01_std_public_filehub.type_id
                                                                                and a.number_id = L01_std_public_filehub.number_id),'N'),
                                        x_file_center_cnt = 
                                                                            (select convert(integer,value)
                                                                            from L01_setup
                                                                            where id = 1),
                                        x_centraldb =
                                        isnull(
                                        (   select count(*)
                                            from L01_std_attestation a
                                            where  L01_std_public_filehub.complex_key = a.sch_no +'_'+convert(varchar,a.year_id)+'_'+convert(varchar,a.sms_id)+'_'+a.cls_id+'_'+a.sub_id+'_'+a.src_dup+'_'+a.emp_id+'_'+a.std_no+'_0' 
                                            and a.attestation_centraldb = 'Y'
                                        ),0),
										content 
                                        FROM L01_std_public_filehub
                                        WHERE complex_key=@complex_key and class_name=@class_name
										and (
												(@class_name = 'StuAttestation' and number_id = @ser_id and (@flag = 'stuattestation' or @flag = 'teaattestation')) or 
												(@flag = 'stuattestationconfirm' and @class_name = 'StuAttestation' and
												 exists(select 1 
														from L01_std_attestation_file a
														join L01_std_attestation b on a.complex_key = b.sch_no +'_'+convert(varchar,b.year_id)+'_'+convert(varchar,b.sms_id)+'_'+b.cls_id+'_'+b.sub_id+'_'+b.src_dup+'_'+b.emp_id+'_'+b.std_no+'_0'
														and b.ser_id = a.number_id
														and isnull(b.attestation_send,'') <>''  
														and isnull(b.attestation_status,'') = 'Y'
														and isnull(b.attestation_release,'') = 'Y'
														where a.complex_key = L01_std_public_filehub.complex_key
														and   a.number_id = L01_std_public_filehub.number_id)) or 
												(@flag = 'stuattestationcentraldb' and @class_name = 'StuAttestation'   and number_id = @ser_id and
												 exists(select 1 
														from L01_std_attestation_file a
														join L01_std_attestation b on a.complex_key = b.sch_no +'_'+convert(varchar,b.year_id)+'_'+convert(varchar,b.sms_id)+'_'+b.cls_id+'_'+b.sub_id+'_'+b.src_dup+'_'+b.emp_id+'_'+b.std_no+'_0'
														and b.ser_id = a.number_id
														and isnull(b.attestation_send,'') <>''  
														and isnull(b.attestation_status,'') = 'Y'
														and isnull(b.attestation_release,'') = 'Y'
														where a.complex_key = L01_std_public_filehub.complex_key
														and   a.number_id = L01_std_public_filehub.number_id
														and   a.check_yn = 'Y')) or 
												(@class_name <> 'StuAttestation')
											)
                                ) AS NewTable
                                ORDER BY file_class, file_extension
                            ";
            //WHERE
            //                        RowNum >= @sRowNun AND RowNum <= @eRowNun


            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QueryAsync<L01_std_public_filehub_DTO>(sqlfile, arg);
            }
        }

        public async Task<L01_std_public_filehub> GetFile(StuFileInfoQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string sqlfile = @"
                                SELECT *
                                FROM L01_std_public_filehub
                                WHERE
                                    class_name=@class_name and type_id=0 and complex_key=@complex_key and number_id=@number_id
                            ";

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<L01_std_public_filehub>(sqlfile, arg);
            }
        }

        public async Task<L01_std_public_filehub> GetFileDownLoad(string arg)
        {
    
            string sqlfile = string.Format(@"
																		select  *
																		from L01_std_public_filehub
																		where file_id = '{0}' ",arg);

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.QuerySingleOrDefaultAsync<L01_std_public_filehub>(sqlfile);
            }
        }

        public async Task<int> InsertFile(IEnumerable<L01_std_public_filehub> files)
        {
            string msg = string.Empty;
            string class_name = string.Empty;
			string[] ary = new string[] { };
            int rt = 0;
            int number_id = 0;
            string sql = @"
                            SELECT max(number_id) 
                            FROM L01_std_public_filehub
                            WHERE class_name=@class_name
                            and type_id = 0 
                            and complex_key=@complex_key
                        ";
			string str_sql = "";
            using (IDbConnection con = _context.CreateCommand())
            {
                con.Open();
                using (IDbTransaction tran = con.BeginTransaction()) 
                {
					try 
					{
                        number_id = await con.ExecuteScalarAsync<int>(sql, new { files.First().complex_key, files.First().class_name }, transaction: tran);

                        foreach (L01_std_public_filehub file in files)
                        {
                            number_id++;
                            file.number_id = number_id;
                            file.upd_dt = updteDate();
                            class_name = file.class_name;
                            ary = file.complex_key.Split('_');
							file.file_id =_schno+";"+ Guid.NewGuid().ToString();
                        }

                        string insert = @"
                                                    INSERT INTO L01_std_public_filehub
                                                    VALUES
                                                    (
                                                        @complex_key, @class_name, @type_id, @number_id, @file_name, @file_extension, @file_blob, @upd_name, @upd_dt,@content,@file_md5,'N',@file_id
                                                    )
                                                    ";

                        await con.ExecuteAsync(insert, files, transaction: tran);

                        if (class_name == "StuAttestation")
                        {
							#region insert into L01_std_attestation
							 str_sql = string.Format(@"
												with temp(sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
													attestation_send,attestation_date,attestation_status,attestation_centraldb,
													content,is_sys,
													credit,grd_id,in_sms_id,borrow_yn,
													reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
													reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name)
												as
												(
												SELECT 
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = 
												convert(varchar(6),case s04_noropenc.noroc_bclass when 'Y' then  --開課班級內碼
														convert(varchar(1),s04_noropenc.dep_id) + 
														case when s04_noropenc.bra_id >= 10 then convert(varchar(2),s04_noropenc.bra_id) else '0' + convert(varchar(1),s04_noropenc.bra_id) end + 
														convert(varchar(1),s04_noropenc.grd_id) + 
														case when s04_noropenc.cls_id >= 10 then convert(varchar(2),s04_noropenc.cls_id) else '0' + convert(varchar(1),s04_noropenc.cls_id) end else convert(varchar(6) , s04_noropenc.noroc_id ) end ),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = {4},
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit = convert(varchar(10),case s90_branch.course_kind when 'C' then s04_noropenc.sub_time when 'F' then s04_noropenc.sub_time else s04_noropenc.sub_credit  end),
												grd_id = convert(smallint,s04_stuhcls.grd_id),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'N',
												reread_yms = '0000',
												reread_type = '0',
												repair_yn  = 'N',
												repair_yms = '0000',
												repair_type = '0',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = s04_stuhcls.year_id,
												actually_sms = s04_stuhcls.sms_id,
												sub_name = s04_108subject.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '1',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = '{0}' and 
													s04_stuhcls.year_id = {1} and 
													s04_stuhcls.sms_id = {2} and
													s04_student.std_no = '{3}'
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
												where s04_student.std_hisdatayear >= 108

												union 

												SELECT
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = convert(varchar(6),s04_noropenc.noroc_id),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = {4},
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit =  convert(varchar , s04_noropenc.sub_credit) ,
												grd_id = convert(varchar(1) , noro_o.grd_id ),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'Y',
												reread_yms = convert(varchar,s04_stuhcls.year_id)+convert(varchar,s04_stuhcls.sms_id),
												reread_type = '1',
												repair_yn  = 'N',
												repair_yms = '0000',
												repair_type = '0',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = convert(smallint , s04_hstusubjscoreterm.in_year ),
												actually_sms = convert(smallint , s04_hstusubjscoreterm.in_sms ),
												sub_name = s04_108subject.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '2',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = '{0}' and 
													s04_stuhcls.year_id = {1} and 
													s04_stuhcls.sms_id = {2} and
													s04_student.std_no = '{3}'
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
												where s04_student.std_hisdatayear >= 108 

												union

												SELECT
												sch_no = s04_student.sch_no,
												year_id = s04_stuhcls.year_id,
												sms_id = s04_stuhcls.sms_id,
												cls_id = convert(varchar(6),s04_noropenc.noroc_id),
												sub_id = convert(varchar(10),s04_subject.sub_id),
												src_dup = convert(varchar(23),s04_stddbgo.course_code),
												emp_id = convert(varchar(50),s04_noropenc.all_empid),
												std_no = s04_student.std_no,
												ser_id = {4},
												attestation_send = '',
												attestation_date = '',
												attestation_status = '',
												attestation_centraldb = 'N',
												content = '',
												is_sys = '2',
												credit = convert(varchar , s04_stddbgo.sub_credit) ,
												grd_id = convert(smallint,s04_stuhcls.grd_id),
												in_sms_id = s04_stuhcls.sms_id,
												borrow_yn = 'N',
												reread_yn = 'N',
												reread_yms = '0000',
												reread_type = '0',
												repair_yn  = 'Y',
												repair_yms = convert(varchar , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 )+convert(varchar , s04_stddbgo.sms_id ),
												repair_type = '1',
												reread2_yn = 'N',
												reread2_yms = '0000',
												turn_yn = 'N',
												upyms = '',
												create_dt = '',
												yms = '0000',
												actually_year = convert(smallint , s04_stddbgo.year_id + s04_stddbgo.grd_id - 1 ),
												actually_sms =  convert(smallint , s04_stddbgo.sms_id ),
												sub_name = subj108_o.sub108_name,
												all_empname = s04_noropenc.all_empname,
												stu_status = '3',
												cls_name = s04_noropenc.noroc_clsname
												FROM s04_student
												inner join s04_stuhcls on
													s04_student.std_no = s04_stuhcls.std_no and												
													s04_stuhcls.sch_no = '{0}' and 
													s04_stuhcls.year_id = {1} and 
													s04_stuhcls.sms_id = {2} and
													s04_student.std_no = '{3}'
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
												where s04_student.std_hisdatayear >= 108
												)
												insert into L01_std_attestation
												(	sch_no,year_id,sms_id,cls_id,sub_id,src_dup,emp_id,std_no,ser_id,
													attestation_send,attestation_date,attestation_status,attestation_centraldb,
													content,is_sys,
													credit,grd_id,in_sms_id,borrow_yn,
													reread_yn,reread_yms,reread_type,repair_yn,repair_yms,repair_type,
													reread2_yn,reread2_yms,turn_yn,upyms,create_dt,yms,actually_year,actually_sms,sub_name,all_empname,stu_status,cls_name)
												select * 
												from temp
												where sch_no+'_'+convert(varchar,year_id)+'_'+convert(varchar,sms_id)+'_'+cls_id+'_'+sub_id+'_'+src_dup+'_'+emp_id+'_'+std_no+'_0' = '{5}'", ary[0], Convert.ToInt64(ary[1]), Convert.ToInt64(ary[2]), ary[7], number_id, files.First().complex_key);
							await con.ExecuteAsync(str_sql, transaction: tran);
							#endregion
						}
						tran.Commit();
						rt = 1;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        rt = 0;
                    }
                }
            }
			return rt;
        }

        public async Task<int> DeleteFile(StuFileInfoQueryList arg)
        {
            arg.sch_no = _context.SchNo;
            string sqlfile = @"
                                DELETE L01_std_public_filehub
                                WHERE class_name=@class_name 
                                and type_id = 0 
                                and complex_key=@complex_key 
                                and number_id=@number_id
                            ";

            using (IDbConnection con = _context.CreateCommand())
            {
                return await con.ExecuteAsync(sqlfile, arg);
            }
        }
    }
}
