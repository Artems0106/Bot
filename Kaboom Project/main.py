import discord
from discord.ext import commands, tasks
from datetime import datetime
from discord.ui import Button, View
import pymongo

# Создаем объект Intents с разрешением на получение всех событий
intents = discord.Intents.all()

# Вставьте свой токен бота здесь
TOKEN = 'MTI2NzYyNDEzMzQ4MzU2NTE5MA.GPFxpU.A0S3XJ6p6jaZzOaYVB9ggN4aBSFzNM2zQIL3Sg'
PREFIX = '!'
bot = commands.Bot(command_prefix=PREFIX, intents=intents)


@bot.event
async def on_ready():
    print(f'{bot.user.name} подключен к Discord!')


# Команда для отправки фото с кнопками
@bot.command(name='фото')
async def send_photo(ctx):
    url = 'https://cdn.discordapp.com/attachments/1269724684820156477/1295113454151143485/2.png'  # Замените на ваш URL фото

    # Создаем кнопки
    button_rules = Button(label='Правила', style=discord.ButtonStyle.blurple, emoji='📖', url='https://docs.google.com/document/d/1CTi5oLFxoohfdWVomhIoFGXlyaaDzQxBhdU8aU4CdrU/edit')
    button_application = Button(label='Заявка', style=discord.ButtonStyle.blurple, emoji='📝', url='https://discord.com/channels/1176904118623813704/1286694636710137889')
    button_reglament = Button(label='Регламент', style=discord.ButtonStyle.blurple, emoji='📜', url='https://docs.google.com/document/d/1k3KMjKxXeGKxkwNoat-DSDfYiwXqDYvrbMDXySfunwg/edit')

    view = View()
    view.add_item(button_rules)
    view.add_item(button_application)
    view.add_item(button_reglament)

    await ctx.send(url, view=view)




import discord
from discord.ext import tasks

ROLE_NAMES = {
    "Medium RP": "🅜 Medium RP",
    "< NoRules >": "<:Norules:1295747370939842715> [NoRules]",
    "< Discord >": "<:Discord:1280031625316532304> [Discord]",
    "Контент-мейкер": "🅚 Контент-мейкер",
    "Разработчик": "🇷 Разработчик"
}

@bot.event
async def on_ready():
    print(f'Logged in as {bot.user}')
    channel = bot.get_channel(1286694636710137889)  # Replace with your channel ID
    await find_or_send_initial_message(channel)
    update_status.start(channel)

async def find_or_send_initial_message(channel):
    global status_message
    async for message in channel.history(limit=100):
        if message.author == bot.user and message.embeds and message.embeds[0].title == "Заявки":
            status_message = message
            break
    else:
        status_message = await send_initial_message(channel)

async def send_initial_message(channel):
    embed = discord.Embed(title="Заявки", color=discord.Color.blue())
    embed.add_field(name="Информация о заявках", value=f"```\nЗаявки могут быть отклонены, из-за следующих причин:\n1. У вас нету 100 часов в SCP:Secret Laboratory.\n2. У вас закрытый профиль в Steam.\n3. Не полная заявка.\n4. Шуточная заявка.\n5. Вам нету 14 лет.\n6. Ваш аккаунт в Discord был создан менее 3-х месяцев назад.\nПримечания:\n1. Если прошло более 3-х дней -> Заявка отклонена.\n2. Знаки 🔓 обозначают возможность пользователей подать заявку в тот или иной отдел.\n\n‣ 🔓 - Заявки открыты.\n‣ 🔒 - Заявки в данный момент закрыты и не рассматриваются.\n\n```", inline=False)

    for role_name, display_name in ROLE_NAMES.items():
        embed.add_field(name=display_name, value="[`00/10` - 🔓]", inline=False)

    view = discord.ui.View()
    
    view.add_item(discord.ui.Button(label="Discord", style=discord.ButtonStyle.primary, emoji="<:Discord:1280031625316532304>", url="https://docs.google.com/forms/d/e/1FAIpQLSfIdPZKtC_WFqBEmWWepI0Btq3zGDENrGkE6tedzmFgnwVtQQ/viewform"))  # Replace with your URL

    return await channel.send(embed=embed, view=view)

@tasks.loop(minutes=1)
async def update_status(channel):
    guild = channel.guild
    embed = discord.Embed(title="Заявки", color=discord.Color.blue())
    embed.add_field(name="Информация о заявках", value=f"```\nЗаявки могут быть отклонены, из-за следующих причин:\n1. У вас нету 100 часов в SCP:Secret Laboratory.\n2. У вас закрытый профиль в Steam.\n3. Не полная заявка.\n4. Шуточная заявка.\n5. Вам нету 14 лет.\n6. Ваш аккаунт в Discord был создан менее 3-х месяцев назад.\nПримечания:\n1. Если прошло более 3-х дней -> Заявка отклонена.\n2. Знаки 🔓 обозначают возможность пользователей подать заявку в тот или иной отдел.```\n\n‣ 🔓 - Заявки открыты.\n‣ 🔒 - Заявки в данный момент закрыты и не рассматриваются.\n\nㅤ\n", inline=False)

    for role_name, display_name in ROLE_NAMES.items():
        role = discord.utils.get(guild.roles, name=role_name)
        if role:
            member_count = sum(1 for member in guild.members if role in member.roles)
              
            if role_name == "< Discord >":
                embed.add_field(name=display_name, value=f"**[`{member_count:02}/08`** - 🔓]", inline=False)
            elif role_name == "< NoRules >":
                embed.add_field(name=display_name, value=f"**[`{member_count:02}/10`** - 🔓]", inline=False)
           
                

    if status_message.author == bot.user:
        view = discord.ui.View()
        view.add_item(discord.ui.Button(label="Discord", style=discord.ButtonStyle.primary, emoji="<:Discord:1280031625316532304>", url="https://docs.google.com/forms/d/e/1FAIpQLSfIdPZKtC_WFqBEmWWepI0Btq3zGDENrGkE6tedzmFgnwVtQQ/viewform"))  # Replace with your URL
        view.add_item(discord.ui.Button(label="NoRules", style=discord.ButtonStyle.primary, emoji="<:Norules:1295747370939842715>", url="https://docs.google.com/forms/d/e/1FAIpQLScH51Kqe1FY4xeqjpoENZyP3oNEllZW2hs_WMM2CcQ-dvDkUw/viewform"))  # Replace with your URL
        await status_message.edit(embed=embed, view=view)




import discord
from discord.ext import commands
from discord.ui import Button, View, Modal, TextInput
from pymongo import MongoClient
from datetime import datetime
import uuid
import pymongo

# MongoDB клиент
mongo_client = MongoClient("mongodb+srv://discord_bot:01062010(Art@discord.5ojz8.mongodb.net/?retryWrites=true&w=majority&appName=discord")
db = mongo_client['discord_bot']
complaints_collection = db['complaints']

intents = discord.Intents.all()


ALLOWED_ROLES = [1208103496268910602, 1208103605127880704]  # ID ролей с доступом


class ComplaintModal(Modal):
    def __init__(self, author, message=None, complaint_id=None):
        super().__init__(title=f"Жалоба от {author.name}#{author.discriminator}")
        self.author = author
        self.message = message
        self.complaint_id = complaint_id

        existing_embed = message.embeds[0] if message else None
        self.add_item(TextInput(label="Место нарушения", placeholder="Название канала",
                                default=existing_embed.fields[0].value if existing_embed else ""))
        self.add_item(TextInput(label="Нарушитель", placeholder="Discord тег или ID",
                                default=existing_embed.fields[1].value if existing_embed else ""))
        self.add_item(TextInput(label="Описание нарушения", style=discord.TextStyle.paragraph,
                                placeholder="Опишите, что произошло",
                                default=existing_embed.fields[2].value if existing_embed else ""))
        self.add_item(TextInput(label="Доказательства", placeholder="Ссылка на доказательства",
                                default=existing_embed.fields[3].value if existing_embed else ""))

    async def on_submit(self, interaction: discord.Interaction):
        embed = self.message.embeds[0] if self.message else discord.Embed(color=discord.Color.blue())
        embed.clear_fields()
        embed.add_field(name="Место нарушения", value=self.children[0].value, inline=False)
        embed.add_field(name="Нарушитель", value=self.children[1].value, inline=False)
        embed.add_field(name="Описание нарушения", value=self.children[2].value, inline=False)
        embed.add_field(name="Доказательства", value=self.children[3].value, inline=False)

        if self.message:
            await self.message.edit(embed=embed)
        else:
            complaint_id = str(uuid.uuid4())
            embed.description = f"**Поступила новая жалоба от {interaction.user.mention}**"
            message = await interaction.channel.send(embed=embed, view=PersistentComplaintView(interaction.user, complaint_id))

            # Сохраняем жалобу в базе данных
            complaints_collection.insert_one({
                "complaint_id": complaint_id,
                "author_id": interaction.user.id,
                "channel_id": interaction.channel.id,
                "message_id": message.id,
                "embed_data": embed.to_dict(),
                "status": "new"
            })

        await interaction.response.send_message("Жалоба успешно отправлена!", ephemeral=True)


class PersistentComplaintView(View):
    def __init__(self, author, complaint_id):
        super().__init__(timeout=None)
        self.author = author
        self.complaint_id = complaint_id
        self.add_item(EditButton(author, complaint_id))
        self.add_item(DeleteButton(author, complaint_id))
        self.add_item(ReviewButton(complaint_id))
        self.add_item(DecisionButton("✅Принять", discord.ButtonStyle.success, "accept", complaint_id))
        self.add_item(DecisionButton("❌Отклонить", discord.ButtonStyle.danger, "reject", complaint_id))


class EditButton(Button):
    def __init__(self, author, complaint_id):
        super().__init__(label="✍️Редактировать", style=discord.ButtonStyle.primary, custom_id=f"edit_{complaint_id}")
        self.author = author
        self.complaint_id = complaint_id

    async def callback(self, interaction: discord.Interaction):
        if interaction.user.id == self.author.id:
            modal = ComplaintModal(self.author, interaction.message, self.complaint_id)
            await interaction.response.send_modal(modal)
        else:
            await interaction.response.send_message("У вас нет прав для редактирования этой жалобы.", ephemeral=True)


class DeleteButton(Button):
    def __init__(self, author, complaint_id):
        super().__init__(label="🗑️Удалить", style=discord.ButtonStyle.danger, custom_id=f"delete_{complaint_id}")
        self.author = author
        self.complaint_id = complaint_id

    async def callback(self, interaction: discord.Interaction):
        if interaction.user.id == self.author.id or interaction.user.guild_permissions.administrator:
            complaints_collection.delete_one({"complaint_id": self.complaint_id})
            await interaction.message.delete()
            await interaction.response.send_message("Жалоба удалена.", ephemeral=True)
        else:
            await interaction.response.send_message("У вас нет прав для удаления этой жалобы.", ephemeral=True)


class ReviewButton(Button):
    def __init__(self, complaint_id):
        super().__init__(label="💡На рассмотрении", style=discord.ButtonStyle.secondary, custom_id=f"review_{complaint_id}")
        self.complaint_id = complaint_id

    async def callback(self, interaction: discord.Interaction):
        if not any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            await interaction.response.send_message("У вас нет прав для рассмотрения этой жалобы.", ephemeral=True)
            return

        embed = interaction.message.embeds[0]
        embed.color = discord.Color.yellow()
        await interaction.message.edit(embed=embed)

        if not interaction.message.thread:
            thread = await interaction.message.create_thread(name="Рассмотрение жалобы")
            await thread.send("Эта ветка создана для обсуждения жалобы.")

        await interaction.response.send_message("Жалоба находится на рассмотрении.", ephemeral=True)


class DecisionButton(Button):
    def __init__(self, label, style, action, complaint_id):
        super().__init__(label=label, style=style, custom_id=f"decision_{action}_{complaint_id}")
        self.action = action
        self.complaint_id = complaint_id

    async def callback(self, interaction: discord.Interaction):
        if not any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            await interaction.response.send_message("У вас нет прав для выполнения этого действия.", ephemeral=True)
            return

        embed = interaction.message.embeds[0]
        decision_time = datetime.now().strftime("%d.%m.%Y %H:%M")

        if self.action == "accept":
            embed.set_footer(text=f"Подозреваемый будет наказан | {decision_time}")
            embed.color = discord.Color.green()
        elif self.action == "reject":
            embed.set_footer(text=f"Жалоба отклонена | {decision_time}")
            embed.color = discord.Color.red()

        complaints_collection.update_one({"complaint_id": self.complaint_id}, {"$set": {"status": self.action}})
        await interaction.message.edit(embed=embed)
        await interaction.response.send_message(
            f"Жалоба {'принята' if self.action == 'accept' else 'отклонена'}.", ephemeral=True
        )


@bot.event
async def on_ready():
    # Восстанавливаем кнопки из базы данных
    for complaint in complaints_collection.find():
        channel = bot.get_channel(complaint["channel_id"])
        if channel:
            try:
                message = await channel.fetch_message(complaint["message_id"])
                view = PersistentComplaintView(
                    author=discord.Object(id=complaint["author_id"]),
                    complaint_id=complaint["complaint_id"]
                )
                await message.edit(view=view)
            except discord.NotFound:
                complaints_collection.delete_one({"complaint_id": complaint["complaint_id"]})

    print(f"Бот {bot.user} запущен и готов к работе!")


@bot.command()
async def complaint(ctx):
    embed = discord.Embed(
        title="Отправить жалобу",
        description="Нажмите кнопку ниже, чтобы отправить жалобу.",
        color=discord.Color.blue()
    )

    view = View(timeout=None)
    button = Button(label="✍️Отправить жалобу", style=discord.ButtonStyle.primary, custom_id="send_complaint")

    async def button_callback(interaction: discord.Interaction):
        modal = ComplaintModal(interaction.user)
        await interaction.response.send_modal(modal)

    button.callback = button_callback
    view.add_item(button)

    await ctx.send(embed=embed, view=view)



import discord
from discord.ext import commands
from discord.ui import Button, View, Modal, TextInput
from datetime import datetime

intents = discord.Intents.all()
intents.messages = True


ALLOWED_ROLES = [1273052608680038461]  # ID ролей с доступом


class SCPComplaintModal(Modal):
    def __init__(self, author, message=None):
        super().__init__(title=f"Жалоба на игрока от {author.name}#{author.discriminator}")
        self.author = author
        self.message = message

        existing_embed = message.embeds[0] if message else None
        self.add_item(TextInput(label="Сервер", placeholder="Название сервера",
                                default=existing_embed.fields[0].value if existing_embed else ""))
        self.add_item(TextInput(label="Нарушитель", placeholder="Discord тег, SteamID или ник",
                                default=existing_embed.fields[1].value if existing_embed else ""))
        self.add_item(TextInput(label="Описание нарушения", style=discord.TextStyle.paragraph,
                                placeholder="Что произошло?",
                                default=existing_embed.fields[2].value if existing_embed else ""))
        self.add_item(TextInput(label="Доказательства", placeholder="Ссылки на скриншоты, видео или логи",
                                default=existing_embed.fields[3].value if existing_embed else ""))

    async def on_submit(self, interaction: discord.Interaction):
        embed = self.message.embeds[0] if self.message else discord.Embed(color=discord.Color.blue())
        embed.clear_fields()
        embed.add_field(name="Сервер", value=self.children[0].value, inline=False)
        embed.add_field(name="Нарушитель", value=self.children[1].value, inline=False)
        embed.add_field(name="Описание нарушения", value=self.children[2].value, inline=False)
        embed.add_field(name="Доказательства", value=self.children[3].value, inline=False)

        if self.message:
            await self.message.edit(embed=embed)
        else:
            embed.description = f"**Поступила новая жалоба на игрока от {interaction.user.mention}**"
            self.message = await interaction.channel.send(embed=embed, view=SCPPersistentComplaintView(self.author))

        await interaction.response.send_message("Жалоба успешно отправлена!", ephemeral=True)


class SCPPersistentComplaintView(View):
    def __init__(self, author):
        super().__init__(timeout=None)
        self.author = author
        self.add_item(SCPEditButton(author))
        self.add_item(SCPDeleteButton(author))
        self.add_item(SCPReviewButton())
        self.add_item(SCPDecisionButton("✅Принять", discord.ButtonStyle.success, "accept", self))
        self.add_item(SCPDecisionButton("❌Отклонить", discord.ButtonStyle.danger, "reject", self))


class SCPEditButton(Button):
    def __init__(self, author):
        super().__init__(label="✍️Редактировать", style=discord.ButtonStyle.primary, custom_id="scp_edit_button")
        self.author = author

    async def callback(self, interaction: discord.Interaction):
        if interaction.user == self.author:
            modal = SCPComplaintModal(self.author, interaction.message)
            await interaction.response.send_modal(modal)
        else:
            await interaction.response.send_message("У вас нет прав для редактирования этой жалобы.", ephemeral=True)


class SCPDeleteButton(Button):
    def __init__(self, author):
        super().__init__(label="🗑️Удалить", style=discord.ButtonStyle.danger, custom_id="scp_delete_button")
        self.author = author

    async def callback(self, interaction: discord.Interaction):
        if interaction.user == self.author or interaction.user.guild_permissions.administrator:
            await interaction.message.delete()
            await interaction.response.send_message("Жалоба удалена.", ephemeral=True)
        else:
            await interaction.response.send_message("У вас нет прав для удаления этой жалобы.", ephemeral=True)


class SCPReviewButton(Button):
    def __init__(self):
        super().__init__(label="💡На рассмотрении", style=discord.ButtonStyle.secondary, custom_id="scp_review_button")

    async def callback(self, interaction: discord.Interaction):
        if not any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            await interaction.response.send_message("У вас нет прав для рассмотрения этой жалобы.", ephemeral=True)
            return

        embed = interaction.message.embeds[0]
        embed.color = discord.Color.yellow()
        await interaction.message.edit(embed=embed)

        if not interaction.message.thread:
            thread = await interaction.message.create_thread(name="Рассмотрение жалобы на игрока")
            await thread.send("Эта ветка создана для обсуждения жалобы.")

        await interaction.response.send_message("Жалоба находится на рассмотрении.", ephemeral=True)


class SCPDecisionButton(Button):
    def __init__(self, label, style, action, parent_view):
        super().__init__(label=label, style=style, custom_id=f"scp_decision_{action}")
        self.action = action
        self.parent_view = parent_view

    async def callback(self, interaction: discord.Interaction):
        if not any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            await interaction.response.send_message("У вас нет прав для выполнения этого действия.", ephemeral=True)
            return

        embed = interaction.message.embeds[0]
        decision_time = datetime.now().strftime("%d.%m.%Y %H:%M")

        if self.action == "accept":
            embed.set_footer(text=f"Игрок будет наказан | {decision_time}")
            embed.color = discord.Color.green()
        elif self.action == "reject":
            embed.set_footer(text=f"Жалоба отклонена | {decision_time}")
            embed.color = discord.Color.red()

        embed.add_field(name="Жалобу рассмотрел", value=f"{interaction.user.mention}")

        for item in self.parent_view.children:
            item.disabled = True

        await interaction.message.edit(embed=embed, view=self.parent_view)
        await interaction.response.send_message(
            f"Жалоба {'принята' if self.action == 'accept' else 'отклонена'}.", ephemeral=True
        )


@bot.command()
async def scp_complaint(ctx):
    embed = discord.Embed(
        title="Жалоба на игрока SCP: Secret Laboratory",
        description="Нажмите кнопку ниже, чтобы подать жалобу на игрока.",
        color=discord.Color.blue()
    )

    view = View(timeout=None)
    button = Button(label="✍️Подать жалобу", style=discord.ButtonStyle.primary, custom_id="scp_send_complaint")

    async def button_callback(interaction: discord.Interaction):
        modal = SCPComplaintModal(interaction.user)
        await interaction.response.send_modal(modal)

    button.callback = button_callback
    view.add_item(button)

    await ctx.send(embed=embed, view=view)




import discord
from discord.ext import commands
from discord.ui import Button, View, Modal, TextInput
from datetime import datetime

intents = discord.Intents.all()
intents.messages = True



class DonatorComplaintModal(Modal):
    def __init__(self, author, message=None):
        super().__init__(title=f"Жалоба на донатора от {author.name}#{author.discriminator}")
        self.author = author
        self.message = message

        existing_embed = message.embeds[0] if message else None
        self.add_item(TextInput(label="Место, где произошло нарушение", placeholder="Название сервера",
                                default=existing_embed.fields[0].value if existing_embed else ""))
        self.add_item(TextInput(label="Нарушитель", placeholder="Никнейм (по возможности SteamID64)",
                                default=existing_embed.fields[1].value if existing_embed else ""))
        self.add_item(TextInput(label="Что было нарушено", placeholder="Номер правила или краткое изложение ситуации",
                                default=existing_embed.fields[2].value if existing_embed else ""))
        self.add_item(TextInput(label="Доказательство", placeholder="Наличие доказательства обязательно",
                                default=existing_embed.fields[3].value if existing_embed else ""))

    async def on_submit(self, interaction: discord.Interaction):
        embed = self.message.embeds[0] if self.message else discord.Embed(color=discord.Color.blue())
        embed.clear_fields()
        embed.add_field(name="Место, где произошло нарушение", value=self.children[0].value, inline=False)
        embed.add_field(name="Нарушитель", value=self.children[1].value, inline=False)
        embed.add_field(name="Что было нарушено", value=self.children[2].value, inline=False)
        embed.add_field(name="Доказательство", value=self.children[3].value, inline=False)

        if self.message:
            await self.message.edit(embed=embed)
        else:
            embed.description = f"**Поступила новая жалоба на донатора от {interaction.user.mention}**"
            self.message = await interaction.channel.send(embed=embed, view=DonatorPersistentComplaintView(self.author))

        await interaction.response.send_message("Жалоба успешно отправлена!", ephemeral=True)


class DonatorPersistentComplaintView(View):
    def __init__(self, author):
        super().__init__(timeout=None)
        self.author = author
        self.add_item(DonatorEditButton(author))
        self.add_item(DonatorDeleteButton(author))
        self.add_item(DonatorReviewButton())
        self.add_item(DonatorDecisionButton("✅Принять", discord.ButtonStyle.success, "accept", self))
        self.add_item(DonatorDecisionButton("❌Отклонить", discord.ButtonStyle.danger, "reject", self))


class DonatorEditButton(Button):
    def __init__(self, author):
        super().__init__(label="✍️Редактировать", style=discord.ButtonStyle.primary, custom_id="donator_edit_button")
        self.author = author

    async def callback(self, interaction: discord.Interaction):
        if interaction.user == self.author:
            modal = DonatorComplaintModal(self.author, interaction.message)
            await interaction.response.send_modal(modal)
        else:
            await interaction.response.send_message("У вас нет прав для редактирования этой жалобы.", ephemeral=True)


class DonatorDeleteButton(Button):
    def __init__(self, author):
        super().__init__(label="🗑️Удалить", style=discord.ButtonStyle.danger, custom_id="donator_delete_button")
        self.author = author

    async def callback(self, interaction: discord.Interaction):
        if interaction.user == self.author or interaction.user.guild_permissions.administrator:
            await interaction.message.delete()
            await interaction.response.send_message("Жалоба удалена.", ephemeral=True)
        else:
            await interaction.response.send_message("У вас нет прав для удаления этой жалобы.", ephemeral=True)


class DonatorReviewButton(Button):
    def __init__(self):
        super().__init__(label="💡На рассмотрении", style=discord.ButtonStyle.secondary, custom_id="donator_review_button")

    async def callback(self, interaction: discord.Interaction):
        embed = interaction.message.embeds[0]
        embed.color = discord.Color.yellow()
        await interaction.message.edit(embed=embed)

        if not interaction.message.thread:
            thread = await interaction.message.create_thread(name="Рассмотрение жалобы на донатора")
            await thread.send("Эта ветка создана для обсуждения жалобы.")

        await interaction.response.send_message("Жалоба находится на рассмотрении.", ephemeral=True)


class DonatorDecisionButton(Button):
    def __init__(self, label, style, action, parent_view):
        super().__init__(label=label, style=style, custom_id=f"donator_decision_{action}")
        self.action = action
        self.parent_view = parent_view

    async def callback(self, interaction: discord.Interaction):
        embed = interaction.message.embeds[0]
        decision_time = datetime.now().strftime("%d.%m.%Y %H:%M")

        if self.action == "accept":
            embed.set_footer(text=f"Донатор будет наказан | {decision_time}")
            embed.color = discord.Color.green()
        elif self.action == "reject":
            embed.set_footer(text=f"Жалоба отклонена | {decision_time}")
            embed.color = discord.Color.red()

        embed.add_field(name="Жалобу рассмотрел", value=f"{interaction.user.mention}")

        for item in self.parent_view.children:
            item.disabled = True

        await interaction.message.edit(embed=embed, view=self.parent_view)
        await interaction.response.send_message(
            f"Жалоба {'принята' if self.action == 'accept' else 'отклонена'}.", ephemeral=True
        )


@bot.command()
async def donator_complaint(ctx):
    embed = discord.Embed(
        title="Жалоба на донатора",
        description="Нажмите кнопку ниже, чтобы подать жалобу на донатора.",
        color=discord.Color.blue()
    )

    view = View(timeout=None)
    button = Button(label="✍️Подать жалобу", style=discord.ButtonStyle.primary, custom_id="donator_send_complaint")

    async def button_callback(interaction: discord.Interaction):
        modal = DonatorComplaintModal(interaction.user)
        await interaction.response.send_modal(modal)

    button.callback = button_callback
    view.add_item(button)

    await ctx.send(embed=embed, view=view)


import discord
from discord.ext import commands
from discord.ui import Button, View, Modal, TextInput
from datetime import datetime

intents = discord.Intents.all()
intents.messages = True

ALLOWED_ROLES = [1273053207148626012, 1208093749666644038]  # ID ролей, которые могут взаимодействовать с жалобами


class LeadershipComplaintModal(Modal):
    def __init__(self, author, message=None):
        super().__init__(title=f"Жалоба на руководство от {author.name}#{author.discriminator}")
        self.author = author
        self.message = message

        existing_embed = message.embeds[0] if message else None
        self.add_item(TextInput(label="Место, где произошло нарушение", placeholder="Например, сервер или канал",
                                default=existing_embed.fields[0].value if existing_embed else ""))
        self.add_item(TextInput(label="Руководитель", placeholder="Имя, тег или ID",
                                default=existing_embed.fields[1].value if existing_embed else ""))
        self.add_item(TextInput(label="Суть жалобы", style=discord.TextStyle.paragraph,
                                placeholder="Опишите проблему или нарушение",
                                default=existing_embed.fields[2].value if existing_embed else ""))
        self.add_item(TextInput(label="Доказательства", placeholder="Приложите ссылки на скриншоты или видео",
                                default=existing_embed.fields[3].value if existing_embed else ""))

    async def on_submit(self, interaction: discord.Interaction):
        embed = self.message.embeds[0] if self.message else discord.Embed(color=discord.Color.blue())
        embed.clear_fields()
        embed.add_field(name="Место, где произошло нарушение", value=self.children[0].value, inline=False)
        embed.add_field(name="Руководитель", value=self.children[1].value, inline=False)
        embed.add_field(name="Суть жалобы", value=self.children[2].value, inline=False)
        embed.add_field(name="Доказательства", value=self.children[3].value, inline=False)

        if self.message:
            await self.message.edit(embed=embed)
        else:
            embed.description = f"**Поступила новая жалоба на руководство от {interaction.user.mention}**"
            self.message = await interaction.channel.send(embed=embed, view=LeadershipPersistentComplaintView())

        await interaction.response.send_message("Жалоба успешно отправлена!", ephemeral=True)


class LeadershipPersistentComplaintView(View):
    def __init__(self):
        super().__init__(timeout=None)
        self.add_item(LeadershipEditButton())
        self.add_item(LeadershipDeleteButton())
        self.add_item(LeadershipReviewButton())
        self.add_item(LeadershipDecisionButton("✅Принять", discord.ButtonStyle.success, "accept", self))
        self.add_item(LeadershipDecisionButton("❌Отклонить", discord.ButtonStyle.danger, "reject", self))


class LeadershipEditButton(Button):
    def __init__(self):
        super().__init__(label="✍️Редактировать", style=discord.ButtonStyle.primary, custom_id="leadership_edit_button")

    async def callback(self, interaction: discord.Interaction):
        modal = LeadershipComplaintModal(interaction.user, interaction.message)
        await interaction.response.send_modal(modal)


class LeadershipDeleteButton(Button):
    def __init__(self):
        super().__init__(label="🗑️Удалить", style=discord.ButtonStyle.danger, custom_id="leadership_delete_button")

    async def callback(self, interaction: discord.Interaction):
        if any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            await interaction.message.delete()
            await interaction.response.send_message("Жалоба удалена.", ephemeral=True)
        else:
            await interaction.response.send_message("У вас нет прав для удаления этой жалобы.", ephemeral=True)


class LeadershipReviewButton(Button):
    def __init__(self):
        super().__init__(label="💡На рассмотрении", style=discord.ButtonStyle.secondary, custom_id="leadership_review_button")

    async def callback(self, interaction: discord.Interaction):
        if any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            embed = interaction.message.embeds[0]
            embed.color = discord.Color.yellow()
            await interaction.message.edit(embed=embed)

            if not interaction.message.thread:
                thread = await interaction.message.create_thread(name="Рассмотрение жалобы на руководство")
                await thread.send("Эта ветка создана для обсуждения жалобы.")

            await interaction.response.send_message("Жалоба находится на рассмотрении.", ephemeral=True)
        else:
            await interaction.response.send_message("У вас нет прав для рассмотрения этой жалобы.", ephemeral=True)


class LeadershipDecisionButton(Button):
    def __init__(self, label, style, action, parent_view):
        super().__init__(label=label, style=style, custom_id=f"leadership_decision_{action}")
        self.action = action
        self.parent_view = parent_view

    async def callback(self, interaction: discord.Interaction):
        if any(role.id in ALLOWED_ROLES for role in interaction.user.roles):
            embed = interaction.message.embeds[0]
            decision_time = datetime.now().strftime("%d.%m.%Y %H:%M")

            if self.action == "accept":
                embed.set_footer(text=f"Жалоба одобрена | {decision_time}")
                embed.color = discord.Color.green()
            elif self.action == "reject":
                embed.set_footer(text=f"Жалоба отклонена | {decision_time}")
                embed.color = discord.Color.red()

            embed.add_field(name="Жалобу рассмотрел", value=f"{interaction.user.mention}")

            for item in self.parent_view.children:
                item.disabled = True

            await interaction.message.edit(embed=embed, view=self.parent_view)
            await interaction.response.send_message(
                f"Жалоба {'принята' if self.action == 'accept' else 'отклонена'}.", ephemeral=True
            )
        else:
            await interaction.response.send_message("У вас нет прав для выполнения этого действия.", ephemeral=True)


@bot.command()
async def leadership_complaint(ctx):
    embed = discord.Embed(
        title="Жалоба на руководство",
        description="Нажмите кнопку ниже, чтобы подать жалобу на руководство.",
        color=discord.Color.blue()
    )

    view = View(timeout=None)
    button = Button(label="✍️Подать жалобу", style=discord.ButtonStyle.primary, custom_id="leadership_send_complaint")

    async def button_callback(interaction: discord.Interaction):
        modal = LeadershipComplaintModal(interaction.user)
        await interaction.response.send_modal(modal)

    button.callback = button_callback
    view.add_item(button)

    await ctx.send(embed=embed, view=view)





# Запуск бота
bot.run(TOKEN)

