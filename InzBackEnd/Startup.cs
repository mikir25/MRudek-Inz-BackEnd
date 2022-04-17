using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InzBackEnd.Authorization;
using InzBackEnd.Entities;
using InzBackEnd.Entities.Conversation;
using InzBackEnd.Entities.Models;
using InzBackEnd.Middleware;
using InzBackEnd.Models;
using InzBackEnd.Models.Validators;
using InzBackEnd.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InzBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddFluentValidation();

            services.AddAutoMapper(this.GetType().Assembly);

            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Event>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Forum>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Gadget>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Post>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Tutorial>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Comment>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Friend>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<UserGroup>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<Mail>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<EditUser>>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler<EditPassword>>();

            services.AddScoped<PortalSeeder>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IGadgetService, GadgetService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ITutorialService, TutorialService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<EditPassword>, EditPasswordValidator>();
            services.AddScoped<IValidator<EditUser>, EditUserValidator>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IGroupConversationService, GroupConversationService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddHttpContextAccessor();
            

            //Authentication
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);
            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });

            //Db Configuration           
            services.AddDbContext<PortalDbContext>
            (opctions =>
            {
                opctions.UseSqlServer(Configuration.GetConnectionString("AppDbConnection"));
            });            

            //Add Cors
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>
                builder.AllowAnyMethod()
                .AllowAnyHeader()
                //.WithOrigins(Configuration.GetConnectionString("URLFrontEnd"))
                .AllowAnyOrigin()
                );
            });

            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Praca", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PortalSeeder seeder)
        {
            app.UseCors("FrontEndClient");
            seeder.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Praca In¿ynierska");
            });

            app.UseRouting();   
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
